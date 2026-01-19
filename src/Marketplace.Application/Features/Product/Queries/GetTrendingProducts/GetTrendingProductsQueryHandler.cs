using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Queries.GetTrendingProducts;

public class GetTrendingProductsQueryHandler : IRequestHandler<GetTrendingProductsQuery, Result<TrendingProductsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTrendingProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TrendingProductsResponse>> Handle(GetTrendingProductsQuery request, CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(request.TimeWindow ?? TimeSpan.FromDays(30));

        var query = _context.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.Status == Domain.Entities.ProductStatus.Published &&
                       p.IsActive &&
                       p.CreatedAt >= cutoffDate) // Only consider recently active products
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // Get products with their base data
        var products = await query.ToListAsync(cancellationToken);

        // Calculate trending scores
        var trendingProducts = products
            .Select(p => new
            {
                Product = p,
                TrendingScore = CalculateTrendingScore(p, request.Algorithm ?? "weighted", cutoffDate),
                VelocityScore = CalculateVelocityScore(p, cutoffDate),
                LastActivity = GetLastActivity(p)
            })
            .Where(x => x.TrendingScore > 0) // Only include products with some trending activity
            .OrderByDescending(x => x.TrendingScore)
            .ThenByDescending(x => x.VelocityScore)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var totalCount = products.Count(p =>
            CalculateTrendingScore(p, request.Algorithm ?? "weighted", cutoffDate) > 0);

        var productDtos = trendingProducts.Select(x =>
        {
            var dto = MapToProductDto(x.Product);
            return new TrendingProductDto
            {
                Id = dto.Id,
                VendorId = dto.VendorId,
                VendorName = dto.VendorName,
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                ShortDescription = dto.ShortDescription,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                Price = dto.Price,
                CompareAtPrice = dto.CompareAtPrice,
                CostPrice = dto.CostPrice,
                StockQuantity = dto.StockQuantity,
                LowStockThreshold = dto.LowStockThreshold,
                TrackInventory = dto.TrackInventory,
                AllowBackorder = dto.AllowBackorder,
                Status = dto.Status,
                IsActive = dto.IsActive,
                IsFeatured = dto.IsFeatured,
                IsDigital = dto.IsDigital,
                RequiresShipping = dto.RequiresShipping,
                Weight = dto.Weight,
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,
                MetaTitle = dto.MetaTitle,
                MetaDescription = dto.MetaDescription,
                MetaKeywords = dto.MetaKeywords,
                Rating = dto.Rating,
                ReviewCount = dto.ReviewCount,
                ViewCount = dto.ViewCount,
                SalesCount = dto.SalesCount,
                ApprovalStatus = dto.ApprovalStatus,
                PublishedAt = dto.PublishedAt,
                Images = dto.Images,
                Variations = dto.Variations,
                Attributes = dto.Attributes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                TrendingScore = x.TrendingScore,
                VelocityScore = x.VelocityScore,
                LastActivity = x.LastActivity
            };
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<TrendingProductsResponse>.Success(
            new TrendingProductsResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            }
        );
    }

    private double CalculateTrendingScore(Domain.Entities.Product product, string algorithm, DateTime cutoffDate)
    {
        var daysSinceCreated = Math.Max(1, (DateTime.UtcNow - product.CreatedAt).TotalDays);
        var daysSinceCutoff = Math.Max(1, (DateTime.UtcNow - cutoffDate).TotalDays);

        return algorithm.ToLower() switch
        {
            "popular" => ((double)product.ViewCount * 0.3 + product.SalesCount * 0.7) / daysSinceCreated,
            "recent" => Math.Max(0, (daysSinceCutoff - daysSinceCreated) / daysSinceCutoff) *
                       (product.ViewCount + product.SalesCount * 2),
            "weighted" => ((double)product.ViewCount * 0.2 + product.SalesCount * 0.5 + (double)product.Rating * product.ReviewCount * 0.3) / Math.Sqrt(daysSinceCreated),
            _ => ((double)product.ViewCount * 0.3 + product.SalesCount * 0.7) / daysSinceCreated
        };
    }

    private double CalculateVelocityScore(Domain.Entities.Product product, DateTime cutoffDate)
    {
        // Simple velocity calculation - in production, you'd track historical data
        var daysActive = Math.Max(1, (DateTime.UtcNow - product.CreatedAt).TotalDays);
        var activityRate = (product.ViewCount + product.SalesCount * 3) / daysActive;

        // Boost score for products with recent activity
        var recencyBonus = product.UpdatedAt.HasValue &&
                          (DateTime.UtcNow - product.UpdatedAt.Value).TotalDays < 7 ? 1.5 : 1.0;

        return activityRate * recencyBonus;
    }

    private DateTime? GetLastActivity(Domain.Entities.Product product)
    {
        return product.UpdatedAt ?? product.CreatedAt;
    }

    private ProductDto MapToProductDto(Domain.Entities.Product product)
    {
        return new ProductDto
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
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}