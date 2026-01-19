using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.ShippingSettings.UpdateShippingSettings;

public class UpdateShippingSettingsCommand : IRequest<Result<ShippingSettingsResponse>>
{
    public bool OffersFreeShipping { get; set; }
    public decimal? FreeShippingThreshold { get; set; }
    public decimal? DefaultShippingCost { get; set; }
    public ShippingMethod DefaultShippingMethod { get; set; }
    public bool AllowLocalPickup { get; set; }
    public decimal? LocalPickupFee { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
    public string? ShippingPolicy { get; set; }
}

public class ShippingSettingsResponse
{
    public Guid Id { get; set; }
    public bool OffersFreeShipping { get; set; }
    public decimal? FreeShippingThreshold { get; set; }
    public decimal? DefaultShippingCost { get; set; }
    public ShippingMethod DefaultShippingMethod { get; set; }
    public bool AllowLocalPickup { get; set; }
    public decimal? LocalPickupFee { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
}
