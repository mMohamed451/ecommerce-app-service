using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<bool>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var category = await _context.Categories
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result<bool>.Failure(
                "Category not found",
                new List<string> { "Category does not exist" }
            );
        }

        // Check if category has children
        if (category.Children.Any())
        {
            return Result<bool>.Failure(
                "Cannot delete category",
                new List<string> { "Cannot delete category that has subcategories. Please delete or move subcategories first." }
            );
        }

        // Check if category has products
        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == request.CategoryId, cancellationToken);

        if (hasProducts)
        {
            return Result<bool>.Failure(
                "Cannot delete category",
                new List<string> { "Cannot delete category that has products. Please remove or reassign products first." }
            );
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Category deleted successfully");
    }
}
