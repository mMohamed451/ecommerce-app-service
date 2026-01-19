using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetTrendingProducts;

public class GetTrendingProductsQuery : IRequest<Result<TrendingProductsResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public TimeSpan? TimeWindow { get; set; } = TimeSpan.FromDays(30); // Trending over last 30 days
    public Guid? CategoryId { get; set; } // Filter by category
    public string? Algorithm { get; set; } = "weighted"; // "weighted", "recent", "popular"
}

public class TrendingProductsResponse
{
    public List<TrendingProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class TrendingProductDto : ProductDto
{
    public double TrendingScore { get; set; }
    public double VelocityScore { get; set; } // Rate of change in views/sales
    public DateTime? LastActivity { get; set; }
}