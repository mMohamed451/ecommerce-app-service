using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<bool>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<bool>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to delete products" }
            );
        }

        // Get product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == vendor.Id, cancellationToken);

        if (product == null)
        {
            return Result<bool>.Failure(
                "Product not found",
                new List<string> { "Product does not exist or you don't have permission to delete it" }
            );
        }

        // Soft delete - set status to Deleted instead of removing from database
        product.Status = ProductStatus.Deleted;
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = userId.Value.ToString();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Product deleted successfully");
    }
}
