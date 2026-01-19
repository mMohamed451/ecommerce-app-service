using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorReviewSummary;

public class GetVendorReviewSummaryQueryHandler : IRequestHandler<GetVendorReviewSummaryQuery, Result<VendorReviewSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetVendorReviewSummaryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<VendorReviewSummaryDto>> Handle(GetVendorReviewSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<VendorReviewSummaryDto>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<VendorReviewSummaryDto>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        // TODO: When Review entity is implemented, replace this with actual rating aggregation
        // For now, using vendor's aggregated rating
        
        var summary = new VendorReviewSummaryDto
        {
            AverageRating = vendor.Rating,
            TotalReviews = vendor.TotalReviews,
            RatingDistribution = new RatingDistributionDto
            {
                // Placeholder - will be calculated from actual Review entities
                Five = 0,
                Four = 0,
                Three = 0,
                Two = 0,
                One = 0
            }
        };

        return Result<VendorReviewSummaryDto>.Success(summary, "Review summary retrieved successfully");
    }
}
