using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.CreateAddress;

public class CreateAddressCommand : IRequest<Result<AddressDto>>
{
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
