using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorProfile;

public class GetVendorProfileQuery : IRequest<Result<VendorDto>>
{
}
