using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorVerification : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum DocumentType
{
    BusinessLicense = 0,
    TaxCertificate = 1,
    IdentityProof = 2,
    BankStatement = 3,
    Other = 4
}

public enum DocumentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
