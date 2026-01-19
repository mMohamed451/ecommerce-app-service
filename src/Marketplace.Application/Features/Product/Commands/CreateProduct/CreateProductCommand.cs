using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<ProductResponse>>
{
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public int StockQuantity { get; set; } = 0;
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
    public List<ProductAttributeInput> Attributes { get; set; } = new();
    public List<ProductVariationInput> Variations { get; set; } = new();
}

public class ProductAttributeInput
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public AttributeType Type { get; set; } = AttributeType.Text;
    public int DisplayOrder { get; set; } = 0;
}

public class ProductVariationInput
{
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; } = 0;
    public decimal? Weight { get; set; }
    public List<ProductVariationAttributeInput> Attributes { get; set; } = new();
}

public class ProductVariationAttributeInput
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ProductStatus Status { get; set; }
    public ProductApprovalStatus ApprovalStatus { get; set; }
}
