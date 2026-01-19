using MediatR;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;

namespace Marketplace.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand : IRequest<Result<CartItemDto>>
{
    public Guid ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public int Quantity { get; init; } = 1;
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}