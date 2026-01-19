using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.Commands.CreateProduct;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Result<ProductResponse>>
{
    public Guid ProductId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorder { get; set; } = false;
    public bool IsDigital { get; set; } = false;
    public bool RequiresShipping { get; set; } = true;
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}
