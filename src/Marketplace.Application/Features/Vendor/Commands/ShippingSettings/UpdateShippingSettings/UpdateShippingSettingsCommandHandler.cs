using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.ShippingSettings.UpdateShippingSettings;

public class UpdateShippingSettingsCommandHandler : IRequestHandler<UpdateShippingSettingsCommand, Result<ShippingSettingsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateShippingSettingsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ShippingSettingsResponse>> Handle(UpdateShippingSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ShippingSettingsResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ShippingSettingsResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        var shippingSettings = await _context.VendorShippingSettings
            .FirstOrDefaultAsync(s => s.VendorId == vendor.Id, cancellationToken);

        if (shippingSettings == null)
        {
            // Create new shipping settings
            shippingSettings = new VendorShippingSettings
            {
                VendorId = vendor.Id,
                CreatedBy = userId.Value.ToString()
            };
            _context.VendorShippingSettings.Add(shippingSettings);
        }

        shippingSettings.OffersFreeShipping = request.OffersFreeShipping;
        shippingSettings.FreeShippingThreshold = request.FreeShippingThreshold;
        shippingSettings.DefaultShippingCost = request.DefaultShippingCost;
        shippingSettings.DefaultShippingMethod = request.DefaultShippingMethod;
        shippingSettings.AllowLocalPickup = request.AllowLocalPickup;
        shippingSettings.LocalPickupFee = request.LocalPickupFee;
        shippingSettings.EstimatedDeliveryDays = request.EstimatedDeliveryDays;
        shippingSettings.ShippingPolicy = request.ShippingPolicy;
        shippingSettings.IsActive = true;
        shippingSettings.UpdatedBy = userId.Value.ToString();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<ShippingSettingsResponse>.Success(new ShippingSettingsResponse
        {
            Id = shippingSettings.Id,
            OffersFreeShipping = shippingSettings.OffersFreeShipping,
            FreeShippingThreshold = shippingSettings.FreeShippingThreshold,
            DefaultShippingCost = shippingSettings.DefaultShippingCost,
            DefaultShippingMethod = shippingSettings.DefaultShippingMethod,
            AllowLocalPickup = shippingSettings.AllowLocalPickup,
            LocalPickupFee = shippingSettings.LocalPickupFee,
            EstimatedDeliveryDays = shippingSettings.EstimatedDeliveryDays
        });
    }
}
