using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Application.Features.Profile.Commands.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, Result<UploadAvatarResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UploadAvatarCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<UploadAvatarResponse>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<UploadAvatarResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return Result<UploadAvatarResponse>.Failure(
                "User not found",
                new List<string> { "User does not exist" }
            );
        }

        // Validate file
        if (request.FileStream == null || request.FileSize == 0)
        {
            return Result<UploadAvatarResponse>.Failure(
                "Invalid file",
                new List<string> { "Avatar file is required" }
            );
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<UploadAvatarResponse>.Failure(
                "Invalid file type",
                new List<string> { "Only JPG, PNG, and WEBP images are allowed" }
            );
        }

        // Validate file size (5MB max)
        if (request.FileSize > 5 * 1024 * 1024)
        {
            return Result<UploadAvatarResponse>.Failure(
                "File too large",
                new List<string> { "File size must be less than 5MB" }
            );
        }

        try
        {
            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                await _fileStorageService.DeleteFileAsync(user.Avatar, cancellationToken);
            }

            // Upload new avatar
            var fileName = $"avatars/{userId.Value}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var avatarUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                fileName,
                request.ContentType,
                cancellationToken);

            // Update user avatar
            user.Avatar = avatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // Rollback: delete uploaded file
                await _fileStorageService.DeleteFileAsync(avatarUrl, cancellationToken);
                var errors = updateResult.Errors.Select(e => e.Description).ToList();
                return Result<UploadAvatarResponse>.Failure("Failed to update avatar", errors);
            }

            return Result<UploadAvatarResponse>.Success(
                new UploadAvatarResponse { AvatarUrl = avatarUrl },
                "Avatar uploaded successfully"
            );
        }
        catch (Exception ex)
        {
            return Result<UploadAvatarResponse>.Failure(
                "Upload failed",
                new List<string> { ex.Message }
            );
        }
    }
}
