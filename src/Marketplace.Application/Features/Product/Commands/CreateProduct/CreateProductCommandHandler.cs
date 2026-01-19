using Marketplace.Application.Common.Helpers;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISearchIndexService _searchIndexService;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ISearchIndexService searchIndexService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _searchIndexService = searchIndexService;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ProductResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor for the current user
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ProductResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to create products" }
            );
        }

        if (!vendor.IsActive || vendor.VerificationStatus != VerificationStatus.Approved)
        {
            return Result<ProductResponse>.Failure(
                "Vendor not approved",
                new List<string> { "Your vendor account must be approved before creating products" }
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

        // Generate unique slug
        var baseSlug = SlugHelper.GenerateSlug(request.Name);
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            baseSlug,
            async (s) => await _context.Products.AnyAsync(p => p.Slug == s, cancellationToken)
        );

        // Check SKU uniqueness if provided
        if (!string.IsNullOrWhiteSpace(request.SKU))
        {
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU == request.SKU && p.VendorId == vendor.Id, cancellationToken);

            if (skuExists)
            {
                return Result<ProductResponse>.Failure(
                    "SKU already exists",
                    new List<string> { "A product with this SKU already exists" }
                );
            }
        }

        // Create product
        var product = new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            VendorId = vendor.Id,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            SKU = request.SKU,
            Barcode = request.Barcode,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            CostPrice = request.CostPrice,
            StockQuantity = request.StockQuantity,
            LowStockThreshold = request.LowStockThreshold,
            TrackInventory = request.TrackInventory,
            AllowBackorder = request.AllowBackorder,
            Status = ProductStatus.Draft,
            IsActive = false, // Will be active after approval
            IsDigital = request.IsDigital,
            RequiresShipping = request.RequiresShipping,
            Weight = request.Weight,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            ApprovalStatus = ProductApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.Value.ToString()
        };

        await _context.Products.AddAsync(product, cancellationToken);

        // Add attributes
        if (request.Attributes != null && request.Attributes.Any())
        {
            var attributes = request.Attributes.Select((attr, index) => new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = attr.Name,
                Value = attr.Value,
                Type = attr.Type,
                DisplayOrder = attr.DisplayOrder > 0 ? attr.DisplayOrder : index,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.Value.ToString()
            }).ToList();

            await _context.ProductAttributes.AddRangeAsync(attributes, cancellationToken);
        }

        // Add variations
        if (request.Variations != null && request.Variations.Any())
        {
            var variations = new List<ProductVariation>();
            foreach (var variationInput in request.Variations)
            {
                var variation = new ProductVariation
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Name = variationInput.Name,
                    SKU = variationInput.SKU,
                    Price = variationInput.Price,
                    StockQuantity = variationInput.StockQuantity,
                    Weight = variationInput.Weight,
                    TrackInventory = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.Value.ToString()
                };

                // Add variation attributes
                if (variationInput.Attributes != null && variationInput.Attributes.Any())
                {
                    var variationAttributes = variationInput.Attributes.Select(attr => new ProductVariationAttribute
                    {
                        Id = Guid.NewGuid(),
                        ProductVariationId = variation.Id,
                        Name = attr.Name,
                        Value = attr.Value
                    }).ToList();

                    variation.VariationAttributes = variationAttributes;
                }

                variations.Add(variation);
            }

            await _context.ProductVariations.AddRangeAsync(variations, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Index product for search (fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await _searchIndexService.IndexProductAsync(product.Id, cancellationToken);
            }
            catch
            {
                // Log error but don't fail the request
            }
        }, cancellationToken);

        return Result<ProductResponse>.Success(
            new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Status = product.Status,
                ApprovalStatus = product.ApprovalStatus
            },
            "Product created successfully"
        );
    }
}
