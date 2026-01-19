using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.UploadLogo;

public class UploadLogoCommandHandler : IRequestHandler<UploadLogoCommand, Result<UploadLogoResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UploadLogoCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<UploadLogoResponse>> Handle(UploadLogoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<UploadLogoResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<UploadLogoResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        // Validate file
        if (request.FileStream == null || request.FileSize == 0)
        {
            return Result<UploadLogoResponse>.Failure(
                "Invalid file",
                new List<string> { "Logo file is required" }
            );
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<UploadLogoResponse>.Failure(
                "Invalid file type",
                new List<string> { "Only JPG, PNG, and WEBP images are allowed" }
            );
        }

        // Validate file size (5MB max)
        if (request.FileSize > 5 * 1024 * 1024)
        {
            return Result<UploadLogoResponse>.Failure(
                "File too large",
                new List<string> { "File size must be less than 5MB" }
            );
        }

        try
        {
            // Delete old logo if exists
            if (!string.IsNullOrEmpty(vendor.Logo))
            {
                await _fileStorageService.DeleteFileAsync(vendor.Logo, cancellationToken);
            }

            // Upload new logo
            var fileName = $"vendor-logos/{vendor.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var logoUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                fileName,
                request.ContentType,
                cancellationToken);

            // Update vendor logo
            vendor.Logo = logoUrl;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = _currentUserService.Email;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<UploadLogoResponse>.Success(
                new UploadLogoResponse { LogoUrl = logoUrl },
                "Logo uploaded successfully"
            );
        }
        catch (Exception ex)
        {
            return Result<UploadLogoResponse>.Failure(
                "Upload failed",
                new List<string> { ex.Message }
            );
        }
    }
}
