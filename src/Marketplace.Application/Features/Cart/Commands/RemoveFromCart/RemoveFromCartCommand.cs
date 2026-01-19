using MediatR;
using Marketplace.Application.Common.Models;

namespace Marketplace.Application.Features.Cart.Commands.RemoveFromCart;

public record RemoveFromCartCommand : IRequest<Result<bool>>
{
    public Guid CartItemId { get; init; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}