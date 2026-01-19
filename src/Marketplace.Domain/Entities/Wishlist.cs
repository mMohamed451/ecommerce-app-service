using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Wishlist : BaseEntity, IAuditable
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    // Basic info
    public string Name { get; set; } = "My Wishlist";
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;

    // Status
    public bool IsActive { get; set; } = true;

    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}