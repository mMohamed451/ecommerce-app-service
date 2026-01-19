using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.UploadCoverImage;

public class UploadCoverImageCommandHandler : IRequestHandler<UploadCoverImageCommand, Result<UploadCoverImageResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UploadCoverImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<UploadCoverImageResponse>> Handle(UploadCoverImageCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<UploadCoverImageResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<UploadCoverImageResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        // Validate file
        if (request.FileStream == null || request.FileSize == 0)
        {
            return Result<UploadCoverImageResponse>.Failure(
                "Invalid file",
                new List<string> { "Cover image file is required" }
            );
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<UploadCoverImageResponse>.Failure(
                "Invalid file type",
                new List<string> { "Only JPG, PNG, and WEBP images are allowed" }
            );
        }

        // Validate file size (10MB max for cover images)
        if (request.FileSize > 10 * 1024 * 1024)
        {
            return Result<UploadCoverImageResponse>.Failure(
                "File too large",
                new List<string> { "File size must be less than 10MB" }
            );
        }

        try
        {
            // Delete old cover image if exists
            if (!string.IsNullOrEmpty(vendor.CoverImage))
            {
                await _fileStorageService.DeleteFileAsync(vendor.CoverImage, cancellationToken);
            }

            // Upload new cover image
            var fileName = $"vendor-covers/{vendor.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var coverImageUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                fileName,
                request.ContentType,
                cancellationToken);

            // Update vendor cover image
            vendor.CoverImage = coverImageUrl;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = _currentUserService.Email;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<UploadCoverImageResponse>.Success(
                new UploadCoverImageResponse { CoverImageUrl = coverImageUrl },
                "Cover image uploaded successfully"
            );
        }
        catch (Exception ex)
        {
            return Result<UploadCoverImageResponse>.Failure(
                "Upload failed",
                new List<string> { ex.Message }
            );
        }
    }
}
