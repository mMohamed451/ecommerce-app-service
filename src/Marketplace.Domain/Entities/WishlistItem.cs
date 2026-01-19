using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class WishlistItem : BaseEntity, IAuditable
{
    public Guid WishlistId { get; set; }
    public Guid ProductId { get; set; }

    // Product snapshot for consistency
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal? ProductCompareAtPrice { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal ProductRating { get; set; } = 0;
    public int ProductReviewCount { get; set; } = 0;

    // Product images (snapshot)
    public string? PrimaryImageUrl { get; set; }
    public string? PrimaryImageAltText { get; set; }

    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual Wishlist? Wishlist { get; set; }
    public virtual Product? Product { get; set; }
}