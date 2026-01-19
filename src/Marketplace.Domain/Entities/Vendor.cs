using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Vendor : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string BusinessEmail { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string? Website { get; set; }
    
    // Address
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    
    // Legal Information
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    
    // Media
    public string? Logo { get; set; }
    public string? CoverImage { get; set; }
    
    // Verification
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    
    // Business Metrics
    public decimal Rating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalSales { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;
    public int TotalProducts { get; set; } = 0;
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool AcceptOrders { get; set; } = true;
    public bool AutoApproveReviews { get; set; } = false;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<VendorVerification> VerificationDocuments { get; set; } = new List<VendorVerification>();
    public virtual ICollection<VendorBankAccount> BankAccounts { get; set; } = new List<VendorBankAccount>();
    public virtual ICollection<VendorCommission> Commissions { get; set; } = new List<VendorCommission>();
    public virtual ICollection<VendorSubscription> Subscriptions { get; set; } = new List<VendorSubscription>();
    public virtual ICollection<VendorLocation> Locations { get; set; } = new List<VendorLocation>();
    public virtual ICollection<VendorBusinessHours> BusinessHours { get; set; } = new List<VendorBusinessHours>();
    public virtual VendorShippingSettings? ShippingSettings { get; set; }
    public virtual ICollection<VendorTaxInfo> TaxInfos { get; set; } = new List<VendorTaxInfo>();
    public virtual ICollection<VendorApiKey> ApiKeys { get; set; } = new List<VendorApiKey>();
    public virtual ICollection<VendorActivityLog> ActivityLogs { get; set; } = new List<VendorActivityLog>();
    public virtual VendorNotificationPreference? NotificationPreference { get; set; }
    public virtual ICollection<VendorPerformanceMetrics> PerformanceMetrics { get; set; } = new List<VendorPerformanceMetrics>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public enum VerificationStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
    Suspended = 4
}
