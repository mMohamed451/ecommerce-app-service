using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.UploadProductImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<ProductImageResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UploadProductImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<ProductImageResponse>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ProductImageResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ProductImageResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to upload product images" }
            );
        }

        // Get product and verify ownership
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == vendor.Id, cancellationToken);

        if (product == null)
        {
            return Result<ProductImageResponse>.Failure(
                "Product not found",
                new List<string> { "Product does not exist or you don't have permission to upload images" }
            );
        }

        try
        {
            // Generate unique file name
            var fileExtension = Path.GetExtension(request.FileName);
            var uniqueFileName = $"products/{product.Id}/{Guid.NewGuid()}{fileExtension}";

            // Upload file to storage
            var fileUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                uniqueFileName,
                request.ContentType,
                cancellationToken);

            // If this is set as primary, unset other primary images
            if (request.IsPrimary)
            {
                var existingPrimaryImages = await _context.ProductImages
                    .Where(i => i.ProductId == product.Id && i.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var img in existingPrimaryImages)
                {
                    img.IsPrimary = false;
                }
            }

            // Get max display order if not specified
            var displayOrder = request.DisplayOrder;
            if (displayOrder == 0)
            {
                var maxOrder = await _context.ProductImages
                    .Where(i => i.ProductId == product.Id)
                    .Select(i => (int?)i.DisplayOrder)
                    .DefaultIfEmpty(0)
                    .MaxAsync(cancellationToken);

                displayOrder = (maxOrder ?? 0) + 1;
            }

            // Create product image entity
            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                FileName = request.FileName,
                FilePath = fileUrl,
                FileUrl = fileUrl,
                ContentType = request.ContentType,
                FileSize = request.FileSize,
                DisplayOrder = displayOrder,
                IsPrimary = request.IsPrimary,
                AltText = request.AltText,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.Value.ToString()
            };

            await _context.ProductImages.AddAsync(productImage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<ProductImageResponse>.Success(
                new ProductImageResponse
                {
                    Id = productImage.Id,
                    FileUrl = productImage.FileUrl,
                    IsPrimary = productImage.IsPrimary
                },
                "Product image uploaded successfully"
            );
        }
        catch (Exception ex)
        {
            return Result<ProductImageResponse>.Failure(
                "Upload failed",
                new List<string> { $"Failed to upload image: {ex.Message}" }
            );
        }
    }
}
