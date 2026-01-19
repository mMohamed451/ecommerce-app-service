using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorLocation : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "Main Store", "Warehouse 1"
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPickupLocation { get; set; }
    public bool IsWarehouse { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}
