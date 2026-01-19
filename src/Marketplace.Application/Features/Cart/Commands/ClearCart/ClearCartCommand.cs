using MediatR;
using Marketplace.Application.Common.Models;

namespace Marketplace.Application.Features.Cart.Commands.ClearCart;

public record ClearCartCommand : IRequest<Result<bool>>
{
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}