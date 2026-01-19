using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.DeactivateVendor;

public class DeactivateVendorCommand : IRequest<Result<bool>>
{
    public bool IsActive { get; set; }
}
