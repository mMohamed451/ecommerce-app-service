using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Queries.SearchVendorsByLocation;

public class SearchVendorsByLocationQuery : IRequest<Result<List<VendorLocationDto>>>
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal RadiusInKm { get; set; } = 10; // Default 10km radius
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class VendorLocationDto
{
    public Guid VendorId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal DistanceInKm { get; set; }
    public bool IsPickupLocation { get; set; }
    public bool IsWarehouse { get; set; }
}
