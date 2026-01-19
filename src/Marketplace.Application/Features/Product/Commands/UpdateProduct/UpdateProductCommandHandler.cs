using Marketplace.Application.Common.Helpers;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.Commands.CreateProduct;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ProductResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ProductResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to update products" }
            );
        }

        // Get product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == vendor.Id, cancellationToken);

        if (product == null)
        {
            return Result<ProductResponse>.Failure(
                "Product not found",
                new List<string> { "Product does not exist or you don't have permission to update it" }
            );
        }

        // Validate category if provided
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId.Value && c.IsActive, cancellationToken);

            if (!categoryExists)
            {
                return Result<ProductResponse>.Failure(
                    "Invalid category",
                    new List<string> { "The specified category does not exist or is not active" }
                );
            }
        }

        // Check SKU uniqueness if changed
        if (!string.IsNullOrWhiteSpace(request.SKU) && request.SKU != product.SKU)
        {
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU == request.SKU && p.VendorId == vendor.Id && p.Id != product.Id, cancellationToken);

            if (skuExists)
            {
                return Result<ProductResponse>.Failure(
                    "SKU already exists",
                    new List<string> { "A product with this SKU already exists" }
                );
            }
        }

        // Update slug if name changed
        if (request.Name != product.Name)
        {
            var baseSlug = SlugHelper.GenerateSlug(request.Name);
            product.Slug = await SlugHelper.GenerateUniqueSlugAsync(
                baseSlug,
                async (s) => await _context.Products.AnyAsync(p => p.Slug == s && p.Id != product.Id, cancellationToken)
            );
        }

        // Update product properties
        product.CategoryId = request.CategoryId;
        product.Name = request.Name;
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.SKU = request.SKU;
        product.Barcode = request.Barcode;
        product.Price = request.Price;
        product.CompareAtPrice = request.CompareAtPrice;
        product.CostPrice = request.CostPrice;
        product.StockQuantity = request.StockQuantity;
        product.LowStockThreshold = request.LowStockThreshold;
        product.TrackInventory = request.TrackInventory;
        product.AllowBackorder = request.AllowBackorder;
        product.IsDigital = request.IsDigital;
        product.RequiresShipping = request.RequiresShipping;
        product.Weight = request.Weight;
        product.Length = request.Length;
        product.Width = request.Width;
        product.Height = request.Height;
        product.MetaTitle = request.MetaTitle;
        product.MetaDescription = request.MetaDescription;
        product.MetaKeywords = request.MetaKeywords;
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = userId.Value.ToString();

        // Reset approval status if significant changes were made
        if (product.ApprovalStatus == ProductApprovalStatus.Approved)
        {
            product.ApprovalStatus = ProductApprovalStatus.UnderReview;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<ProductResponse>.Success(
            new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Status = product.Status,
                ApprovalStatus = product.ApprovalStatus
            },
            "Product updated successfully"
        );
    }
}
