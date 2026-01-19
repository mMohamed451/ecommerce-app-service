using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetProducts;

public class GetProductsQuery : IRequest<Result<ProductsListResponse>>
{
    public Guid? VendorId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public Domain.Entities.ProductStatus? Status { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } // "name", "price", "created", "rating"
    public bool SortDescending { get; set; } = false;
}

public class ProductsListResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
