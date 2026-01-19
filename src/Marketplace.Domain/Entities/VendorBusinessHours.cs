using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorBusinessHours : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
    public bool Is24Hours { get; set; }
    public string? Notes { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}
