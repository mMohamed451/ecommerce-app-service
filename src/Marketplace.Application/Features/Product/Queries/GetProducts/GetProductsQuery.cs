using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetProducts;

public class GetProductsQuery : IRequest<Result<ProductsListResponse>>
{
    // Basic Filters
    public Guid? VendorId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public Domain.Entities.ProductStatus? Status { get; set; } = Domain.Entities.ProductStatus.Published;
    public bool? IsActive { get; set; } = true;
    public bool? IsFeatured { get; set; }
    public bool? IsDigital { get; set; }
    public bool? RequiresShipping { get; set; }

    // Price Filters
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Rating Filters
    public decimal? MinRating { get; set; }
    public int? MinReviewCount { get; set; }

    // Location Filters (for vendor location-based search)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusInKm { get; set; } // Search radius for vendor locations

    // Stock Filters
    public bool? InStock { get; set; } // true = stock > 0, false = stock = 0, null = all
    public int? MinStockQuantity { get; set; }

    // Category Hierarchy Filters
    public List<Guid>? CategoryIds { get; set; } // Include subcategories

    // Attribute Filters (for product variations/attributes)
    public Dictionary<string, List<string>>? AttributeFilters { get; set; } // e.g., {"Color": ["Red", "Blue"], "Size": ["M", "L"]}

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Cursor { get; set; } // For cursor-based pagination

    // Sorting
    public string? SortBy { get; set; } // "name", "price", "created", "rating", "popularity", "relevance", "trending"
    public bool SortDescending { get; set; } = false;
}

public class ProductsListResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? NextCursor { get; set; }
}
