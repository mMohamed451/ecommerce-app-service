using MediatR;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;

namespace Marketplace.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public record UpdateCartItemQuantityCommand : IRequest<Result<CartItemDto>>
{
    public Guid CartItemId { get; init; }
    public int Quantity { get; init; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}