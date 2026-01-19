using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Cart : BaseEntity, IAuditable
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    // Cart totals
    public decimal Subtotal { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal ShippingAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;

    // Currency (default to USD)
    public string Currency { get; set; } = "USD";

    // Status
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }

    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}