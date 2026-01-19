using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.DeleteAddress;

public class DeleteAddressCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
