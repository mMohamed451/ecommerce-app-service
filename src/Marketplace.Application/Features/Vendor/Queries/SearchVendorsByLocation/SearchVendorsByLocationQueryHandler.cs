using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Queries.SearchVendorsByLocation;

public class SearchVendorsByLocationQueryHandler : IRequestHandler<SearchVendorsByLocationQuery, Result<List<VendorLocationDto>>>
{
    private readonly IApplicationDbContext _context;

    public SearchVendorsByLocationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VendorLocationDto>>> Handle(SearchVendorsByLocationQuery request, CancellationToken cancellationToken)
    {
        // Get all vendor locations within the radius
        // Using Haversine formula for distance calculation
        var locations = await _context.VendorLocations
            .Include(l => l.Vendor)
            .Where(l => 
                l.IsActive && 
                l.Latitude.HasValue && 
                l.Longitude.HasValue &&
                l.Vendor != null &&
                l.Vendor.IsActive &&
                l.Vendor.VerificationStatus == Domain.Entities.VerificationStatus.Approved)
            .ToListAsync(cancellationToken);

        // Calculate distances and filter by radius
        var locationsWithDistance = locations
            .Select(l => new
            {
                Location = l,
                Distance = CalculateDistance(
                    request.Latitude,
                    request.Longitude,
                    l.Latitude!.Value,
                    l.Longitude!.Value)
            })
            .Where(x => x.Distance <= (double)request.RadiusInKm)
            .OrderBy(x => x.Distance)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new VendorLocationDto
            {
                VendorId = x.Location.VendorId,
                BusinessName = x.Location.Vendor?.BusinessName ?? string.Empty,
                LocationId = x.Location.Id,
                LocationName = x.Location.Name,
                City = x.Location.City,
                State = x.Location.State,
                Country = x.Location.Country,
                Latitude = x.Location.Latitude!.Value,
                Longitude = x.Location.Longitude!.Value,
                DistanceInKm = (decimal)Math.Round(x.Distance, 2),
                IsPickupLocation = x.Location.IsPickupLocation,
                IsWarehouse = x.Location.IsWarehouse
            })
            .ToList();

        return Result<List<VendorLocationDto>>.Success(locationsWithDistance);
    }

    /// <summary>
    /// Calculate distance between two coordinates using Haversine formula
    /// Returns distance in kilometers
    /// </summary>
    private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
