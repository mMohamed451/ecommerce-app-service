using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.TrackProductView;

public class TrackProductViewCommandHandler : IRequestHandler<TrackProductViewCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public TrackProductViewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(TrackProductViewCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result<bool>.Failure("Product not found");
        }

        // Increment view count
        product.ViewCount++;

        // You could also create a ProductViewLog entity to track detailed view analytics
        // For now, we'll just increment the counter

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}