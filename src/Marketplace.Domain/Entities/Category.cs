using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Category : BaseEntity, IAuditable
{
    public Guid? ParentId { get; set; }
    
    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    
    // Display
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Hierarchy
    public string? Path { get; set; } // e.g., "/electronics/computers/laptops"
    public int Level { get; set; } = 0; // 0 = root, 1 = first level, etc.
    
    // Statistics
    public int ProductCount { get; set; } = 0;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
