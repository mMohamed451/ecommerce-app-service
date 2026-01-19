using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.ApproveVendor;

public class ApproveVendorCommand : IRequest<Result<VendorDto>>
{
    public Guid VendorId { get; set; }
    public string? Notes { get; set; }
}
