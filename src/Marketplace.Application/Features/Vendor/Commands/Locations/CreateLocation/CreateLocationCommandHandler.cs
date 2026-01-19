using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.Locations.CreateLocation;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Result<LocationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<LocationResponse>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<LocationResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<LocationResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // If this is set as primary, unset other primary locations
        if (request.IsPrimary)
        {
            var existingPrimary = await _context.VendorLocations
                .Where(l => l.VendorId == vendor.Id && l.IsPrimary)
                .ToListAsync(cancellationToken);

            foreach (var existingLocation in existingPrimary)
            {
                existingLocation.IsPrimary = false;
            }
        }

        var newLocation = new VendorLocation
        {
            VendorId = vendor.Id,
            Name = request.Name,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Phone = request.Phone,
            Email = request.Email,
            IsPrimary = request.IsPrimary,
            IsPickupLocation = request.IsPickupLocation,
            IsWarehouse = request.IsWarehouse,
            IsActive = true,
            CreatedBy = userId.Value.ToString()
        };

        _context.VendorLocations.Add(newLocation);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<LocationResponse>.Success(new LocationResponse
        {
            Id = newLocation.Id,
            Name = newLocation.Name,
            City = newLocation.City,
            State = newLocation.State,
            Country = newLocation.Country,
            IsPrimary = newLocation.IsPrimary,
            IsPickupLocation = newLocation.IsPickupLocation,
            IsWarehouse = newLocation.IsWarehouse
        });
    }
}
