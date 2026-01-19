using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorBankAccount : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string? SwiftCode { get; set; }
    public string? Iban { get; set; }
    public string Currency { get; set; } = "USD";
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}
