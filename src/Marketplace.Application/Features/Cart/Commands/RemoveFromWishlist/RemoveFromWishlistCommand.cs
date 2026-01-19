using MediatR;
using Marketplace.Application.Common.Models;

namespace Marketplace.Application.Features.Cart.Commands.RemoveFromWishlist;

public record RemoveFromWishlistCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; init; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
}