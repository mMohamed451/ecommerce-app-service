using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorActivityLog : BaseEntity
{
    public Guid VendorId { get; set; }
    public ActivityType ActivityType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; } // e.g., "Product", "Order", "Profile"
    public Guid? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON string
    public string? NewValues { get; set; } // JSON string
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Guid? PerformedBy { get; set; } // User ID who performed the action
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum ActivityType
{
    Created = 0,
    Updated = 1,
    Deleted = 2,
    Approved = 3,
    Rejected = 4,
    Suspended = 5,
    Activated = 6,
    Login = 7,
    Logout = 8,
    PasswordChanged = 9,
    ProfileUpdated = 10,
    DocumentUploaded = 11,
    PaymentReceived = 12,
    OrderFulfilled = 13,
    Other = 99
}
