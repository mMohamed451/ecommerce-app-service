using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.Locations.CreateLocation;

public class CreateLocationCommand : IRequest<Result<LocationResponse>>
{
    public string Name { get; set; } = string.Empty;
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
    public bool IsPickupLocation { get; set; }
    public bool IsWarehouse { get; set; }
}

public class LocationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsPickupLocation { get; set; }
    public bool IsWarehouse { get; set; }
}
