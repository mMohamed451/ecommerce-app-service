using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Result<CartItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCartItemQuantityCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CartItemDto>> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the cart item
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductVariation)
                    .ThenInclude(pv => pv.VariationAttributes)
                .FirstOrDefaultAsync(ci => ci.Id == request.CartItemId, cancellationToken);

            if (cartItem == null)
            {
                return Result<CartItemDto>.Failure("Cart item not found");
            }

            // Verify cart ownership
            if (!IsCartOwner(cartItem.Cart, request.UserId, request.SessionId))
            {
                return Result<CartItemDto>.Failure("Access denied to cart item");
            }

            // If quantity is 0 or less, remove the item
            if (request.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<CartItemDto>.Success(null!); // Item was removed
            }

            // Check stock availability
            var availableStock = cartItem.Product?.StockQuantity ?? 0;
            if (cartItem.ProductVariation != null)
            {
                availableStock = cartItem.ProductVariation.StockQuantity;
            }

            if (request.Quantity > availableStock)
            {
                return Result<CartItemDto>.Failure($"Requested quantity exceeds available stock of {availableStock}");
            }

            // Update quantity and total price
            cartItem.Quantity = request.Quantity;
            cartItem.TotalPrice = cartItem.UnitPrice * request.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            cartItem.UpdatedBy = _currentUserService.UserId?.ToString();

            await _context.SaveChangesAsync(cancellationToken);

            // Recalculate cart totals
            await RecalculateCartTotalsAsync(cartItem.CartId, cancellationToken);

            return Result<CartItemDto>.Success(MapToCartItemDto(cartItem, cartItem.ProductVariation));
        }
        catch (Exception ex)
        {
            return Result<CartItemDto>.Failure($"Failed to update cart item quantity: {ex.Message}");
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

    private CartItemDto MapToCartItemDto(CartItem cartItem, ProductVariation? selectedVariation)
    {
        var selectedVariationDto = selectedVariation != null ? new SelectedVariationDto
        {
            Id = selectedVariation.Id,
            Name = selectedVariation.Name,
            Sku = selectedVariation.SKU,
            Price = selectedVariation.Price,
            Attributes = selectedVariation.VariationAttributes.Select(attr => new VariationAttributeDto
            {
                Name = attr.Name,
                Value = attr.Value
            }).ToList()
        } : null;

        return new CartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductVariationId = cartItem.ProductVariationId,
            ProductName = cartItem.ProductName,
            ProductSlug = cartItem.ProductSlug,
            ProductSku = cartItem.ProductSku,
            VendorName = cartItem.VendorName,
            UnitPrice = cartItem.UnitPrice,
            Quantity = cartItem.Quantity,
            TotalPrice = cartItem.TotalPrice,
            PrimaryImageUrl = cartItem.PrimaryImageUrl,
            PrimaryImageAltText = cartItem.PrimaryImageAltText,
            SelectedVariation = selectedVariationDto,
            AddedAt = cartItem.CreatedAt
        };
    }
}