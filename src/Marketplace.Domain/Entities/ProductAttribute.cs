using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class ProductAttribute : BaseEntity, IAuditable
{
    public Guid ProductId { get; set; }
    
    // Attribute Information
    public string Name { get; set; } = string.Empty; // e.g., "Color", "Size", "Material"
    public string Value { get; set; } = string.Empty; // e.g., "Red", "Large", "Cotton"
    public string? DisplayName { get; set; } // Optional display name
    public int DisplayOrder { get; set; } = 0;
    
    // Type
    public AttributeType Type { get; set; } = AttributeType.Text;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Product? Product { get; set; }
}

public enum AttributeType
{
    Text = 0,
    Number = 1,
    Boolean = 2,
    Date = 3,
    Color = 4,
    Image = 5
}
