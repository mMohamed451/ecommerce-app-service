using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.SetDefaultAddress;

public class SetDefaultAddressCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
