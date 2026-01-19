using MediatR;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;

namespace Marketplace.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery : IRequest<Result<CartDto>>
{
    public Guid? UserId { get; init; }
    public string? SessionId { get; init; }
}