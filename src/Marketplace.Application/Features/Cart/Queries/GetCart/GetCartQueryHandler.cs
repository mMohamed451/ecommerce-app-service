using MediatR;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Cart.DTOs;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCartQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await GetCartAsync(request.UserId, request.SessionId, cancellationToken);

            if (cart == null)
            {
                // Return empty cart if none exists
                return Result<CartDto>.Success(new CartDto
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    IsActive = true,
                    Currency = "USD",
                    Subtotal = 0,
                    TaxAmount = 0,
                    ShippingAmount = 0,
                    DiscountAmount = 0,
                    TotalAmount = 0,
                    Items = new List<CartItemDto>(),
                    CreatedAt = DateTime.UtcNow
                });
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                SessionId = cart.SessionId,
                Subtotal = cart.Subtotal,
                TaxAmount = cart.TaxAmount,
                ShippingAmount = cart.ShippingAmount,
                DiscountAmount = cart.DiscountAmount,
                TotalAmount = cart.TotalAmount,
                Currency = cart.Currency,
                IsActive = cart.IsActive,
                ExpiresAt = cart.ExpiresAt,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.Items.Select(MapToCartItemDto).ToList()
            };

            return Result<CartDto>.Success(cartDto);
        }
        catch (Exception ex)
        {
            return Result<CartDto>.Failure($"Failed to retrieve cart: {ex.Message}");
        }
    }

    private async Task<Domain.Entities.Cart?> GetCartAsync(Guid? userId, string? sessionId, CancellationToken cancellationToken)
    {
        if (userId.HasValue)
        {
            return await _context.Carts
                .Include(c => c.Items.OrderBy(i => i.CreatedAt))
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive, cancellationToken);
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            return await _context.Carts
                .Include(c => c.Items.OrderBy(i => i.CreatedAt))
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive, cancellationToken);
        }

        return null;
    }

    private CartItemDto MapToCartItemDto(CartItem cartItem)
    {
        SelectedVariationDto? selectedVariationDto = null;

        if (!string.IsNullOrEmpty(cartItem.SelectedVariationAttributes))
        {
            try
            {
                var attributes = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(cartItem.SelectedVariationAttributes);
                if (attributes != null && cartItem.SelectedVariationName != null)
                {
                    selectedVariationDto = new SelectedVariationDto
                    {
                        Id = cartItem.ProductVariationId ?? Guid.Empty,
                        Name = cartItem.SelectedVariationName,
                        Sku = cartItem.SelectedVariationSku,
                        Price = cartItem.SelectedVariationPrice,
                        Attributes = attributes.Select(attr => new VariationAttributeDto
                        {
                            Name = (string)attr.Name,
                            Value = (string)attr.Value
                        }).ToList()
                    };
                }
            }
            catch
            {
                // If deserialization fails, continue without variation details
            }
        }

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