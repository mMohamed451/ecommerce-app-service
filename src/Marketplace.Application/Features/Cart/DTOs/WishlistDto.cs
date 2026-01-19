using Marketplace.Domain.Entities;

namespace Marketplace.Application.Features.Cart.DTOs;

public class WishlistDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }

    public string Name { get; set; } = "My Wishlist";
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsActive { get; set; }

    // Items
    public List<WishlistItemDto> Items { get; set; } = new();

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    // Product snapshot
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal? ProductCompareAtPrice { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal ProductRating { get; set; }
    public int ProductReviewCount { get; set; }

    // Product images
    public string? PrimaryImageUrl { get; set; }
    public string? PrimaryImageAltText { get; set; }

    // Timestamps
    public DateTime AddedAt { get; set; }
}