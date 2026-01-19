using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Queries.GetWishlist;

public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, Result<List<WishlistItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetWishlistQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WishlistItemDto>>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var wishlist = await GetWishlistAsync(request.UserId, request.SessionId, cancellationToken);

            if (wishlist == null)
            {
                return Result<List<WishlistItemDto>>.Success(new List<WishlistItemDto>());
            }

            var wishlistItemDtos = wishlist.Items
                .OrderByDescending(item => item.CreatedAt)
                .Select(MapToWishlistItemDto)
                .ToList();

            return Result<List<WishlistItemDto>>.Success(wishlistItemDtos);
        }
        catch (Exception ex)
        {
            return Result<List<WishlistItemDto>>.Failure($"Failed to retrieve wishlist: {ex.Message}");
        }
    }

    private async Task<Wishlist?> GetWishlistAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        if (userId.HasValue)
        {
            return await _context.Wishlists
                .Include(w => w.Items.OrderByDescending(i => i.CreatedAt))
                .FirstOrDefaultAsync(w => w.UserId == userId && w.IsActive, cancellationToken);
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            return await _context.Wishlists
                .Include(w => w.Items.OrderByDescending(i => i.CreatedAt))
                .FirstOrDefaultAsync(w => w.SessionId == sessionId && w.IsActive, cancellationToken);
        }

        return null;
    }

    private WishlistItemDto MapToWishlistItemDto(WishlistItem wishlistItem)
    {
        return new WishlistItemDto
        {
            Id = wishlistItem.Id,
            ProductId = wishlistItem.ProductId,
            ProductName = wishlistItem.ProductName,
            ProductSlug = wishlistItem.ProductSlug,
            ProductPrice = wishlistItem.ProductPrice,
            ProductCompareAtPrice = wishlistItem.ProductCompareAtPrice,
            VendorName = wishlistItem.VendorName,
            ProductRating = wishlistItem.ProductRating,
            ProductReviewCount = wishlistItem.ProductReviewCount,
            PrimaryImageUrl = wishlistItem.PrimaryImageUrl,
            PrimaryImageAltText = wishlistItem.PrimaryImageAltText,
            AddedAt = wishlistItem.CreatedAt
        };
    }
}