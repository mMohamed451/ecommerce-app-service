using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFromWishlistCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the wishlist
            var wishlist = await GetWishlistAsync(request.UserId, request.SessionId, cancellationToken);
            if (wishlist == null)
            {
                return Result<bool>.Success(true); // Wishlist doesn't exist, so item is "removed"
            }

            // Find and remove the wishlist item
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.Id && wi.ProductId == request.ProductId, cancellationToken);

            if (wishlistItem == null)
            {
                return Result<bool>.Success(true); // Item doesn't exist, so it's "removed"
            }

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to remove item from wishlist: {ex.Message}");
        }
    }

    private async Task<Wishlist?> GetWishlistAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        if (userId.HasValue)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.IsActive, cancellationToken);
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.SessionId == sessionId && w.IsActive, cancellationToken);
        }

        return null;
    }
}