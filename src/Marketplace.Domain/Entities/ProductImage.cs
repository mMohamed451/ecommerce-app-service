using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class ProductImage : BaseEntity, IAuditable
{
    public Guid ProductId { get; set; }
    
    // Image Information
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    // Display
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
    public string? AltText { get; set; }
    
    // Dimensions
    public int? Width { get; set; }
    public int? Height { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Product? Product { get; set; }
}
