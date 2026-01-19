using Marketplace.Domain.Entities;

namespace Marketplace.Application.Features.Cart.DTOs;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    // Cart totals
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";

    // Status
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Items
    public List<CartItemDto> Items { get; set; } = new();

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }

    // Product snapshot
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }

    // Product images
    public string? PrimaryImageUrl { get; set; }
    public string? PrimaryImageAltText { get; set; }

    // Selected variation
    public SelectedVariationDto? SelectedVariation { get; set; }

    // Timestamps
    public DateTime AddedAt { get; set; }
}

public class SelectedVariationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
    public List<VariationAttributeDto> Attributes { get; set; } = new();
}

public class VariationAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CartSummaryDto
{
    public int ItemCount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
}