using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .Include(p => p.Variations.Where(v => v.IsActive))
                .ThenInclude(v => v.VariationAttributes)
            .Include(p => p.Attributes.OrderBy(a => a.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.Status != Domain.Entities.ProductStatus.Deleted, cancellationToken);

        if (product == null)
        {
            return Result<ProductDto>.Failure(
                "Product not found",
                new List<string> { "Product does not exist" }
            );
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            VendorId = product.VendorId,
            VendorName = product.Vendor?.BusinessName ?? string.Empty,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            TrackInventory = product.TrackInventory,
            AllowBackorder = product.AllowBackorder,
            Status = product.Status,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            IsDigital = product.IsDigital,
            RequiresShipping = product.RequiresShipping,
            Weight = product.Weight,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            MetaTitle = product.MetaTitle,
            MetaDescription = product.MetaDescription,
            MetaKeywords = product.MetaKeywords,
            Rating = product.Rating,
            ReviewCount = product.ReviewCount,
            ViewCount = product.ViewCount,
            SalesCount = product.SalesCount,
            ApprovalStatus = product.ApprovalStatus,
            PublishedAt = product.PublishedAt,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                FileUrl = i.FileUrl,
                AltText = i.AltText,
                DisplayOrder = i.DisplayOrder,
                IsPrimary = i.IsPrimary
            }).ToList(),
            Variations = product.Variations.Select(v => new ProductVariationDto
            {
                Id = v.Id,
                Name = v.Name,
                SKU = v.SKU,
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                ImageUrl = v.ImageUrl,
                IsActive = v.IsActive,
                Attributes = v.VariationAttributes.Select(va => new ProductVariationAttributeDto
                {
                    Name = va.Name,
                    Value = va.Value
                }).ToList()
            }).ToList(),
            Attributes = product.Attributes.Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                Name = a.Name,
                Value = a.Value,
                Type = a.Type,
                DisplayOrder = a.DisplayOrder
            }).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Result<ProductDto>.Success(productDto);
    }
}
