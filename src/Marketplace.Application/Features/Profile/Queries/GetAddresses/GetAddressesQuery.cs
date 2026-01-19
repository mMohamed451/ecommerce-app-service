using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Profile.Queries.GetAddresses;

public class GetAddressesQuery : IRequest<Result<List<AddressDto>>>
{
}
