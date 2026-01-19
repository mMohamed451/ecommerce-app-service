using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Commands.ApproveProduct;

public class ApproveProductCommandHandler : IRequestHandler<ApproveProductCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<Domain.Entities.ApplicationUser> _userManager;

    public ApproveProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<Domain.Entities.ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ApproveProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<bool>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Check if user is admin
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return Result<bool>.Failure(
                "User not found",
                new List<string> { "User does not exist" }
            );
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (!isAdmin)
        {
            return Result<bool>.Failure(
                "Unauthorized",
                new List<string> { "Only administrators can approve or reject products" }
            );
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result<bool>.Failure(
                "Product not found",
                new List<string> { "Product does not exist" }
            );
        }

        if (request.Approve)
        {
            // Approve product
            product.ApprovalStatus = ProductApprovalStatus.Approved;
            product.ApprovedAt = DateTime.UtcNow;
            product.ApprovedBy = user.Email;
            product.RejectionReason = null;

            // If product is in draft status, publish it
            if (product.Status == ProductStatus.Draft)
            {
                product.Status = ProductStatus.Published;
                product.IsActive = true;
                product.PublishedAt = DateTime.UtcNow;
            }
            else if (product.Status == ProductStatus.Published)
            {
                product.IsActive = true;
            }
        }
        else
        {
            // Reject product
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                return Result<bool>.Failure(
                    "Rejection reason required",
                    new List<string> { "Rejection reason is required when rejecting a product" }
                );
            }

            product.ApprovalStatus = ProductApprovalStatus.Rejected;
            product.RejectionReason = request.RejectionReason;
            product.IsActive = false;
        }

        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = user.Email;

        await _context.SaveChangesAsync(cancellationToken);

        var message = request.Approve
            ? "Product approved successfully"
            : "Product rejected successfully";

        return Result<bool>.Success(true, message);
    }
}
