using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class ProductVariationAttribute : BaseEntity
{
    public Guid ProductVariationId { get; set; }
    
    // Attribute Information
    public string Name { get; set; } = string.Empty; // e.g., "Color", "Size"
    public string Value { get; set; } = string.Empty; // e.g., "Red", "Large"
    
    // Navigation property
    public virtual ProductVariation? ProductVariation { get; set; }
}
