using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Commands.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<CartItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddToCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CartItemDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get or create cart
            var cart = await GetOrCreateCartAsync(request.UserId, request.SessionId, cancellationToken);
            if (cart == null)
            {
                return Result<CartItemDto>.Failure("Unable to create or retrieve cart");
            }

            // Get product details
            var product = await _context.Products
                .Include(p => p.Vendor)
                .Include(p => p.Images)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.VariationAttributes)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.Status == ProductStatus.Published && p.IsActive, cancellationToken);

            if (product == null)
            {
                return Result<CartItemDto>.Failure("Product not found or not available");
            }

            // Check stock availability
            var availableStock = product.StockQuantity;
            var selectedVariation = request.ProductVariationId.HasValue
                ? product.Variations.FirstOrDefault(v => v.Id == request.ProductVariationId.Value && v.IsActive)
                : null;

            if (selectedVariation != null)
            {
                availableStock = selectedVariation.StockQuantity;
            }

            if (availableStock < request.Quantity)
            {
                return Result<CartItemDto>.Failure($"Insufficient stock. Available: {availableStock}");
            }

            // Check if item already exists in cart
            var existingCartItem = cart.Items.FirstOrDefault(item =>
                item.ProductId == request.ProductId &&
                item.ProductVariationId == request.ProductVariationId);

            if (existingCartItem != null)
            {
                // Update quantity
                var newQuantity = existingCartItem.Quantity + request.Quantity;
                if (newQuantity > availableStock)
                {
                    return Result<CartItemDto>.Failure($"Cannot add {request.Quantity} more items. Total would exceed available stock of {availableStock}");
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.TotalPrice = existingCartItem.UnitPrice * newQuantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<CartItemDto>.Success(MapToCartItemDto(existingCartItem, selectedVariation));
            }
            else
            {
                // Create new cart item
                var unitPrice = selectedVariation?.Price ?? product.Price;
                var totalPrice = unitPrice * request.Quantity;

                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    ProductVariationId = request.ProductVariationId,
                    ProductName = product.Name,
                    ProductSlug = product.Slug,
                    ProductSku = product.SKU,
                    VendorName = product.Vendor?.BusinessName ?? "Unknown Vendor",
                    UnitPrice = unitPrice,
                    Quantity = request.Quantity,
                    TotalPrice = totalPrice,
                    PrimaryImageUrl = product.Images.FirstOrDefault(img => img.IsPrimary)?.FileUrl,
                    PrimaryImageAltText = product.Images.FirstOrDefault(img => img.IsPrimary)?.AltText,
                    SelectedVariationName = selectedVariation?.Name,
                    SelectedVariationSku = selectedVariation?.SKU?.ToString(),
                    SelectedVariationPrice = selectedVariation?.Price,
                    SelectedVariationAttributes = selectedVariation != null
                        ? System.Text.Json.JsonSerializer.Serialize(selectedVariation.VariationAttributes.Select(attr =>
                            new { attr.Name, attr.Value }).ToList())
                        : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _currentUserService.UserId?.ToString()?.ToString()
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<CartItemDto>.Success(MapToCartItemDto(cartItem, selectedVariation));
            }
        }
        catch (Exception ex)
        {
            return Result<CartItemDto>.Failure($"Failed to add item to cart: {ex.Message}");
        }
    }

    private async Task<Domain.Entities.Cart?> GetOrCreateCartAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart? cart = null;

        if (userId.HasValue)
        {
            // Try to find active cart for user
            cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive, cancellationToken);
        }

        if (cart == null && !string.IsNullOrEmpty(sessionId))
        {
            // Try to find active cart for session
            cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive, cancellationToken);
        }

        if (cart == null)
        {
            // Create new cart
            cart = new Domain.Entities.Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = sessionId,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30), // Cart expires in 30 days
                Currency = "USD",
                Subtotal = 0,
                TaxAmount = 0,
                ShippingAmount = 0,
                DiscountAmount = 0,
                TotalAmount = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId?.ToString()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return cart;
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