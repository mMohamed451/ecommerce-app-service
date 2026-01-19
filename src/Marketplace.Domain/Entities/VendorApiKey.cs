using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorApiKey : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty; // Hashed
    public string? ApiSecret { get; set; } // Hashed
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? LastUsedIp { get; set; }
    public string[]? AllowedIps { get; set; }
    public string[]? Permissions { get; set; } // e.g., ["read:products", "write:orders"]
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}
