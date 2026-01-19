using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class CartItem : BaseEntity, IAuditable
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }

    // Product snapshot for consistency
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice { get; set; }

    // Product images (snapshot)
    public string? PrimaryImageUrl { get; set; }
    public string? PrimaryImageAltText { get; set; }

    // Selected variation details (if any)
    public string? SelectedVariationName { get; set; }
    public string? SelectedVariationSku { get; set; }
    public decimal? SelectedVariationPrice { get; set; }
    public string? SelectedVariationAttributes { get; set; } // JSON string of attributes

    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual Cart? Cart { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ProductVariation? ProductVariation { get; set; }
}