using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorCommission : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal? FixedFee { get; set; }
    public decimal? MinimumCommission { get; set; }
    public decimal? MaximumCommission { get; set; }
    public CommissionType CommissionType { get; set; } = CommissionType.Percentage;
    public bool IsActive { get; set; } = true;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum CommissionType
{
    Percentage = 0,
    Fixed = 1,
    Hybrid = 2
}
