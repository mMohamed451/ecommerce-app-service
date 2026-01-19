using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorSubscription : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public string? PaymentMethodId { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum SubscriptionPlan
{
    Basic = 0,
    Standard = 1,
    Premium = 2,
    Enterprise = 3
}

public enum SubscriptionStatus
{
    Active = 0,
    Cancelled = 1,
    Expired = 2,
    Suspended = 3
}
