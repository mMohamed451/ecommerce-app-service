using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.UpdateVendorProfile;

public class UpdateVendorProfileCommand : IRequest<Result<VendorDto>>
{
    public string? BusinessName { get; set; }
    public string? BusinessDescription { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessPhone { get; set; }
    public string? Website { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
}
