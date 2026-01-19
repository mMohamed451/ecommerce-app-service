using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class ProductVariation : BaseEntity, IAuditable
{
    public Guid ProductId { get; set; }
    
    // Variation Identification
    public string Name { get; set; } = string.Empty; // e.g., "Small - Red", "Large - Blue"
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    
    // Pricing
    public decimal? Price { get; set; } // Override product price if set
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    
    // Inventory
    public int StockQuantity { get; set; } = 0;
    public int? LowStockThreshold { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorder { get; set; } = false;
    
    // Physical Attributes
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    
    // Image
    public string? ImageUrl { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual Product? Product { get; set; }
    public virtual ICollection<ProductVariationAttribute> VariationAttributes { get; set; } = new List<ProductVariationAttribute>();
}
