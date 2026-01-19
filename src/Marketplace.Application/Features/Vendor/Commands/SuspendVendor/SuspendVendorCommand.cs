using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.SuspendVendor;

public class SuspendVendorCommand : IRequest<Result<VendorDto>>
{
    public Guid VendorId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsPermanent { get; set; } = false;
}
