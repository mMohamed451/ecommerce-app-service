using MediatR;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;

namespace Marketplace.Application.Features.Cart.Queries.GetWishlist;

public record GetWishlistQuery : IRequest<Result<List<WishlistItemDto>>>
{
    public Guid? UserId { get; init; }
    public string? SessionId { get; init; }
}