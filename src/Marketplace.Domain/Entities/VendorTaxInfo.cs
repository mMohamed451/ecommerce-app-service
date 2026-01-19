using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorTaxInfo : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public string TaxId { get; set; } = string.Empty; // EIN, VAT, etc.
    public TaxIdType TaxIdType { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    public decimal? TaxRate { get; set; } // For vendors who handle their own tax
    public bool CollectsTax { get; set; } = false;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum TaxIdType
{
    EIN = 0,        // Employer Identification Number (US)
    VAT = 1,        // Value Added Tax (EU/UK)
    GST = 2,        // Goods and Services Tax (AU/CA)
    SSN = 3,        // Social Security Number (US - individual)
    Other = 4
}
