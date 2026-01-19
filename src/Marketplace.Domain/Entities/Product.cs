using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Product : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public Guid? CategoryId { get; set; }
    
    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    
    // Pricing
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    
    // Inventory
    public int StockQuantity { get; set; } = 0;
    public int? LowStockThreshold { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorder { get; set; } = false;
    
    // Status & Visibility
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public bool IsActive { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public bool IsDigital { get; set; } = false;
    public bool RequiresShipping { get; set; } = true;
    
    // Physical Attributes
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Ratings & Reviews
    public decimal Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public int SalesCount { get; set; } = 0;
    
    // Approval (for C2C marketplace)
    public ProductApprovalStatus ApprovalStatus { get; set; } = ProductApprovalStatus.Pending;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectionReason { get; set; }
    
    // Publishing
    public DateTime? PublishedAt { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual Vendor? Vendor { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariation> Variations { get; set; } = new List<ProductVariation>();
    public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
}

public enum ProductStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deleted = 3
}

public enum ProductApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    UnderReview = 3
}
