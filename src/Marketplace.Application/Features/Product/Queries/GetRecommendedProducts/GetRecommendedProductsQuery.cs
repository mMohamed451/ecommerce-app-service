using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetRecommendedProducts;

public class GetRecommendedProductsQuery : IRequest<Result<RecommendedProductsResponse>>
{
    public string? UserId { get; set; } // For personalized recommendations
    public Guid? ProductId { get; set; } // Get recommendations similar to this product
    public Guid? CategoryId { get; set; } // Get recommendations within category
    public int MaxRecommendations { get; set; } = 10;
    public string Algorithm { get; set; } = "collaborative"; // "collaborative", "content-based", "popular"
}

public class RecommendedProductsResponse
{
    public List<RecommendedProductDto> Products { get; set; } = new();
    public string RecommendationType { get; set; } = string.Empty; // e.g., "similar_products", "popular_in_category", "trending"
}

public class RecommendedProductDto : ProductDto
{
    public double SimilarityScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty; // e.g., "Similar customers also bought", "Popular in category"
}