using Marketplace.Application.Common.Helpers;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Category.Commands.CreateCategory;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<CategoryResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result<CategoryResponse>.Failure(
                "Category not found",
                new List<string> { "Category does not exist" }
            );
        }

        // Prevent circular reference - check if new parent is a descendant
        if (request.ParentId.HasValue && request.ParentId.Value == category.Id)
        {
            return Result<CategoryResponse>.Failure(
                "Invalid parent",
                new List<string> { "A category cannot be its own parent" }
            );
        }

        if (request.ParentId.HasValue)
        {
            var parentCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parentCategory == null)
            {
                return Result<CategoryResponse>.Failure(
                    "Parent category not found",
                    new List<string> { "The specified parent category does not exist" }
                );
            }

            // Check for circular reference by checking if parent is a descendant
            var isDescendant = await IsDescendantAsync(request.ParentId.Value, category.Id, cancellationToken);
            if (isDescendant)
            {
                return Result<CategoryResponse>.Failure(
                    "Circular reference",
                    new List<string> { "Cannot set parent category as it would create a circular reference" }
                );
            }

            category.ParentId = request.ParentId;
            category.Level = parentCategory.Level + 1;
            category.Path = string.IsNullOrEmpty(parentCategory.Path)
                ? $"/{parentCategory.Slug}"
                : $"{parentCategory.Path}/{parentCategory.Slug}";
        }
        else if (request.ParentId.HasValue == false && category.ParentId.HasValue)
        {
            // Removing parent - make it root level
            category.ParentId = null;
            category.Level = 0;
            category.Path = null;
        }

        // Update slug if name changed
        if (request.Name != category.Name)
        {
            var baseSlug = SlugHelper.GenerateSlug(request.Name);
            category.Slug = await SlugHelper.GenerateUniqueSlugAsync(
                baseSlug,
                async (s) => await _context.Categories.AnyAsync(c => c.Slug == s && c.Id != category.Id, cancellationToken)
            );
        }

        // Update other properties
        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.Icon = request.Icon;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.IsFeatured = request.IsFeatured;
        category.MetaTitle = request.MetaTitle;
        category.MetaDescription = request.MetaDescription;
        category.MetaKeywords = request.MetaKeywords;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = userId.Value.ToString();

        // Update path for all descendants if parent changed
        if (request.ParentId != category.ParentId)
        {
            await UpdateDescendantsPathAsync(category.Id, category.Path, category.Level, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<CategoryResponse>.Success(
            new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId
            },
            "Category updated successfully"
        );
    }

    private async Task<bool> IsDescendantAsync(Guid potentialParentId, Guid categoryId, CancellationToken cancellationToken)
    {
        var current = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == potentialParentId, cancellationToken);

        while (current != null && current.ParentId.HasValue)
        {
            if (current.ParentId.Value == categoryId)
            {
                return true;
            }
            current = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == current.ParentId.Value, cancellationToken);
        }

        return false;
    }

    private async Task UpdateDescendantsPathAsync(Guid parentId, string? parentPath, int parentLevel, CancellationToken cancellationToken)
    {
        var children = await _context.Categories
            .Where(c => c.ParentId == parentId)
            .ToListAsync(cancellationToken);

        foreach (var child in children)
        {
            child.Level = parentLevel + 1;
            child.Path = string.IsNullOrEmpty(parentPath)
                ? $"/{child.Slug}"
                : $"{parentPath}/{child.Slug}";

            // Recursively update descendants
            await UpdateDescendantsPathAsync(child.Id, child.Path, child.Level, cancellationToken);
        }
    }
}
