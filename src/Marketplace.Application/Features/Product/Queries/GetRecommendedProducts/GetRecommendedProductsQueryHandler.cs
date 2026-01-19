using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Queries.GetRecommendedProducts;

public class GetRecommendedProductsQueryHandler : IRequestHandler<GetRecommendedProductsQuery, Result<RecommendedProductsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetRecommendedProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecommendedProductsResponse>> Handle(GetRecommendedProductsQuery request, CancellationToken cancellationToken)
    {
        var recommendations = new List<RecommendedProductDto>();
        var recommendationType = "popular";

        if (request.ProductId.HasValue)
        {
            // Get similar products
            recommendations = await GetSimilarProducts(request.ProductId.Value, request.MaxRecommendations, cancellationToken);
            recommendationType = "similar_products";
        }
        else if (!string.IsNullOrEmpty(request.UserId))
        {
            // Get personalized recommendations based on user behavior
            // For now, fall back to popular products - in production, you'd analyze user history
            recommendations = await GetPopularProducts(request.MaxRecommendations, request.CategoryId, cancellationToken);
            recommendationType = "personalized";
        }
        else if (request.CategoryId.HasValue)
        {
            // Get popular products in category
            recommendations = await GetPopularProductsInCategory(request.CategoryId.Value, request.MaxRecommendations, cancellationToken);
            recommendationType = "popular_in_category";
        }
        else
        {
            // Get generally popular products
            recommendations = await GetPopularProducts(request.MaxRecommendations, null, cancellationToken);
            recommendationType = "trending";
        }

        return Result<RecommendedProductsResponse>.Success(
            new RecommendedProductsResponse
            {
                Products = recommendations,
                RecommendationType = recommendationType
            }
        );
    }

    private async Task<List<RecommendedProductDto>> GetSimilarProducts(Guid productId, int maxRecommendations, CancellationToken cancellationToken)
    {
        // Get the target product
        var targetProduct = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (targetProduct == null)
        {
            return new List<RecommendedProductDto>();
        }

        // Find similar products based on category, vendor, and attributes
        var similarProducts = await _context.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.Id != productId &&
                       p.Status == Domain.Entities.ProductStatus.Published &&
                       p.IsActive)
            .ToListAsync(cancellationToken);

        var recommendations = similarProducts
            .Select(p => new
            {
                Product = p,
                SimilarityScore = CalculateSimilarityScore(targetProduct, p)
            })
            .Where(x => x.SimilarityScore > 0)
            .OrderByDescending(x => x.SimilarityScore)
            .Take(maxRecommendations)
            .Select(x => MapToRecommendedProductDto(x.Product, x.SimilarityScore, "Similar to the product you're viewing"))
            .ToList();

        return recommendations;
    }

    private async Task<List<RecommendedProductDto>> GetPopularProducts(int maxRecommendations, Guid? categoryId, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.Status == Domain.Entities.ProductStatus.Published && p.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var products = await query
            .OrderByDescending(p => p.SalesCount * 0.7 + p.ViewCount * 0.3) // Weighted popularity score
            .Take(maxRecommendations)
            .ToListAsync(cancellationToken);

        return products
            .Select(p => MapToRecommendedProductDto(p, CalculatePopularityScore(p), "Popular product"))
            .ToList();
    }

    private async Task<List<RecommendedProductDto>> GetPopularProductsInCategory(Guid categoryId, int maxRecommendations, CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.CategoryId == categoryId &&
                       p.Status == Domain.Entities.ProductStatus.Published &&
                       p.IsActive)
            .OrderByDescending(p => p.SalesCount * 0.6 + p.ViewCount * 0.3 + (double)p.Rating * 0.1)
            .Take(maxRecommendations)
            .ToListAsync(cancellationToken);

        return products
            .Select(p => MapToRecommendedProductDto(p, CalculatePopularityScore(p), "Popular in this category"))
            .ToList();
    }

    private double CalculateSimilarityScore(Domain.Entities.Product target, Domain.Entities.Product candidate)
    {
        var score = 0.0;

        // Same category bonus
        if (target.CategoryId == candidate.CategoryId)
        {
            score += 50;
        }

        // Same vendor bonus (people might like other products from same vendor)
        if (target.VendorId == candidate.VendorId)
        {
            score += 30;
        }

        // Price similarity (products in similar price range)
        var priceDiff = Math.Abs(target.Price - candidate.Price);
        var avgPrice = (target.Price + candidate.Price) / 2;
        var priceSimilarity = Math.Max(0, 1 - (double)(priceDiff / avgPrice));
        score += priceSimilarity * 20;

        // Rating similarity
        var ratingDiff = Math.Abs(target.Rating - candidate.Rating);
        score += Math.Max(0, 10 - (double)ratingDiff * 2);

        return score;
    }

    private double CalculatePopularityScore(Domain.Entities.Product product)
    {
        return product.SalesCount * 0.7 + product.ViewCount * 0.3;
    }

    private RecommendedProductDto MapToRecommendedProductDto(Domain.Entities.Product product, double score, string reason)
    {
        var dto = new ProductDto
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

        return new RecommendedProductDto
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
            SimilarityScore = score,
            RecommendationReason = reason
        };
    }
}