using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorShippingSettings : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    public bool OffersFreeShipping { get; set; }
    public decimal? FreeShippingThreshold { get; set; }
    public decimal? DefaultShippingCost { get; set; }
    public ShippingMethod DefaultShippingMethod { get; set; }
    public bool AllowLocalPickup { get; set; }
    public decimal? LocalPickupFee { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
    public string? ShippingPolicy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum ShippingMethod
{
    Standard = 0,
    Express = 1,
    Overnight = 2,
    Economy = 3,
    LocalDelivery = 4
}
