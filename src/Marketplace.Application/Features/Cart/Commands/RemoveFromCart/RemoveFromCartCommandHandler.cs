using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.RemoveFromCart;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFromCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the cart item
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == request.CartItemId, cancellationToken);

            if (cartItem == null)
            {
                return Result<bool>.Failure("Cart item not found");
            }

            // Verify cart ownership
            if (!IsCartOwner(cartItem.Cart, request.UserId, request.SessionId))
            {
                return Result<bool>.Failure("Access denied to cart item");
            }

            // Remove the item
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(cancellationToken);

            // Recalculate cart totals
            await RecalculateCartTotalsAsync(cartItem.CartId, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to remove item from cart: {ex.Message}");
        }
    }

    private bool IsCartOwner(Domain.Entities.Cart cart, Guid? userId, string? sessionId)
    {
        if (userId.HasValue && cart.UserId == userId)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(sessionId) && cart.SessionId == sessionId)
        {
            return true;
        }

        return false;
    }

    private async Task RecalculateCartTotalsAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

        if (cart != null)
        {
            cart.Subtotal = cart.Items.Sum(item => item.TotalPrice);
            cart.TotalAmount = cart.Subtotal + cart.TaxAmount + cart.ShippingAmount - cart.DiscountAmount;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}