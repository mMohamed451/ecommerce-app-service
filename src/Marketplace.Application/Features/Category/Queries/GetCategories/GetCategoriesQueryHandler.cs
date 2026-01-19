using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Category.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Category.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsQueryable();

        // Apply filters
        if (request.ParentId.HasValue)
        {
            query = query.Where(c => c.ParentId == request.ParentId.Value);
        }
        else if (request.ParentId == null)
        {
            // If ParentId is explicitly null, get root categories
            query = query.Where(c => c.ParentId == null);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(c => c.IsFeatured == request.IsFeatured.Value);
        }

        var categories = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var categoryDtos = categories.Select(c => MapToDto(c)).ToList();

        // Build hierarchical structure if requested
        if (request.IncludeChildren && !request.ParentId.HasValue)
        {
            categoryDtos = BuildHierarchy(categoryDtos);
        }

        // Update product counts
        foreach (var dto in categoryDtos)
        {
            dto.ProductCount = await _context.Products
                .CountAsync(p => p.CategoryId == dto.Id && p.Status != Domain.Entities.ProductStatus.Deleted, cancellationToken);
        }

        return Result<List<CategoryDto>>.Success(categoryDtos);
    }

    private List<CategoryDto> BuildHierarchy(List<CategoryDto> allCategories)
    {
        var categoryMap = allCategories.ToDictionary(c => c.Id);
        var rootCategories = new List<CategoryDto>();

        foreach (var category in allCategories)
        {
            if (category.ParentId == null)
            {
                rootCategories.Add(category);
            }
            else if (categoryMap.TryGetValue(category.ParentId.Value, out var parent))
            {
                if (parent.Children == null)
                {
                    parent.Children = new List<CategoryDto>();
                }
                parent.Children.Add(category);
            }
        }

        return rootCategories;
    }

    private CategoryDto MapToDto(Domain.Entities.Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            ParentId = category.ParentId,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            Icon = category.Icon,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            IsFeatured = category.IsFeatured,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            MetaKeywords = category.MetaKeywords,
            Path = category.Path,
            Level = category.Level,
            ProductCount = category.ProductCount,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
