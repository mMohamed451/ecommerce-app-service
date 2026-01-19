using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ClearCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the cart
            var cart = await GetCartAsync(request.UserId, request.SessionId, cancellationToken);
            if (cart == null)
            {
                return Result<bool>.Success(true); // Cart doesn't exist, so it's "cleared"
            }

            // Remove all cart items
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync(cancellationToken);

            _context.CartItems.RemoveRange(cartItems);

            // Reset cart totals
            cart.Subtotal = 0;
            cart.TaxAmount = 0;
            cart.ShippingAmount = 0;
            cart.DiscountAmount = 0;
            cart.TotalAmount = 0;
            cart.UpdatedAt = DateTime.UtcNow;
            cart.UpdatedBy = _currentUserService.UserId?.ToString();

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to clear cart: {ex.Message}");
        }
    }

    private async Task<Domain.Entities.Cart?> GetCartAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        if (userId.HasValue)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive, cancellationToken);
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive, cancellationToken);
        }

        return null;
    }
}