using Marketplace.Application.Common.Helpers;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<CategoryResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Validate parent category if provided
        Domain.Entities.Category? parentCategory = null;
        int level = 0;
        string? path = null;

        if (request.ParentId.HasValue)
        {
            parentCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parentCategory == null)
            {
                return Result<CategoryResponse>.Failure(
                    "Parent category not found",
                    new List<string> { "The specified parent category does not exist" }
                );
            }

            level = parentCategory.Level + 1;
            path = string.IsNullOrEmpty(parentCategory.Path)
                ? $"/{parentCategory.Slug}"
                : $"{parentCategory.Path}/{parentCategory.Slug}";
        }

        // Generate unique slug
        var baseSlug = SlugHelper.GenerateSlug(request.Name);
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            baseSlug,
            async (s) => await _context.Categories.AnyAsync(c => c.Slug == s, cancellationToken)
        );

        // Create category
        var category = new Domain.Entities.Category
        {
            Id = Guid.NewGuid(),
            ParentId = request.ParentId,
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Icon = request.Icon,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            IsFeatured = request.IsFeatured,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            Path = path,
            Level = level,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.Value.ToString()
        };

        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CategoryResponse>.Success(
            new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId
            },
            "Category created successfully"
        );
    }
}
