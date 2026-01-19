using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.UpdateInventory;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, Result<InventoryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateInventoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<InventoryResponse>> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<InventoryResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<InventoryResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to update inventory" }
            );
        }

        // Get product and verify ownership
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == vendor.Id, cancellationToken);

        if (product == null)
        {
            return Result<InventoryResponse>.Failure(
                "Product not found",
                new List<string> { "Product does not exist or you don't have permission to update it" }
            );
        }

        // Update inventory properties
        if (request.StockQuantity.HasValue)
        {
            product.StockQuantity = request.StockQuantity.Value;
        }

        if (request.LowStockThreshold.HasValue)
        {
            product.LowStockThreshold = request.LowStockThreshold.Value;
        }

        if (request.TrackInventory.HasValue)
        {
            product.TrackInventory = request.TrackInventory.Value;
        }

        if (request.AllowBackorder.HasValue)
        {
            product.AllowBackorder = request.AllowBackorder.Value;
        }

        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = userId.Value.ToString();

        await _context.SaveChangesAsync(cancellationToken);

        // Calculate stock status
        var isLowStock = product.TrackInventory &&
                        product.LowStockThreshold.HasValue &&
                        product.StockQuantity <= product.LowStockThreshold.Value &&
                        product.StockQuantity > 0;

        var isOutOfStock = product.TrackInventory && product.StockQuantity <= 0 && !product.AllowBackorder;

        return Result<InventoryResponse>.Success(
            new InventoryResponse
            {
                ProductId = product.Id,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold,
                TrackInventory = product.TrackInventory,
                AllowBackorder = product.AllowBackorder,
                IsLowStock = isLowStock,
                IsOutOfStock = isOutOfStock
            },
            "Inventory updated successfully"
        );
    }
}
