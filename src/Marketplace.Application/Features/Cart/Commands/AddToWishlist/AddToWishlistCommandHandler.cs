using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Result<WishlistItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddToWishlistCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<WishlistItemDto>> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get or create wishlist
            var wishlist = await GetOrCreateWishlistAsync(request.UserId, request.SessionId, cancellationToken);
            if (wishlist == null)
            {
                return Result<WishlistItemDto>.Failure("Unable to create or retrieve wishlist");
            }

            // Check if product is already in wishlist
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.Id && wi.ProductId == request.ProductId, cancellationToken);

            if (existingItem != null)
            {
                return Result<WishlistItemDto>.Failure("Product is already in wishlist");
            }

            // Get product details
            var product = await _context.Products
                .Include(p => p.Vendor)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.Status == ProductStatus.Published && p.IsActive, cancellationToken);

            if (product == null)
            {
                return Result<WishlistItemDto>.Failure("Product not found or not available");
            }

            // Create wishlist item
            var wishlistItem = new WishlistItem
            {
                Id = Guid.NewGuid(),
                WishlistId = wishlist.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSlug = product.Slug,
                ProductPrice = product.Price,
                ProductCompareAtPrice = product.CompareAtPrice,
                VendorName = product.Vendor?.BusinessName ?? "Unknown Vendor",
                ProductRating = product.Rating,
                ProductReviewCount = product.ReviewCount,
                PrimaryImageUrl = product.Images.FirstOrDefault(img => img.IsPrimary)?.FileUrl,
                PrimaryImageAltText = product.Images.FirstOrDefault(img => img.IsPrimary)?.AltText,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId?.ToString()
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<WishlistItemDto>.Success(MapToWishlistItemDto(wishlistItem));
        }
        catch (Exception ex)
        {
            return Result<WishlistItemDto>.Failure($"Failed to add item to wishlist: {ex.Message}");
        }
    }

    private async Task<Wishlist?> GetOrCreateWishlistAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        Wishlist? wishlist = null;

        if (userId.HasValue)
        {
            // Try to find active wishlist for user
            wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.IsActive, cancellationToken);
        }

        if (wishlist == null && !string.IsNullOrEmpty(sessionId))
        {
            // Try to find active wishlist for session
            wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.SessionId == sessionId && w.IsActive, cancellationToken);
        }

        if (wishlist == null)
        {
            // Create new wishlist
            wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = sessionId,
                Name = "My Wishlist",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId?.ToString()
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return wishlist;
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