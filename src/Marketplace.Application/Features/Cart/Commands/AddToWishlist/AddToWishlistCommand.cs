using MediatR;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;

namespace Marketplace.Application.Features.Cart.Commands.AddToWishlist;

public record AddToWishlistCommand : IRequest<Result<WishlistItemDto>>
{
    public Guid ProductId { get; init; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}