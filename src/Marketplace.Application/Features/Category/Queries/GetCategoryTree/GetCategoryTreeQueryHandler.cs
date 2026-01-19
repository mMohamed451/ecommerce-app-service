using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Category.Queries.GetCategoryTree;

public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, Result<CategoryTreeResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryTreeResponse>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        // Get all categories that match the criteria
        var categoriesQuery = _context.Categories.AsQueryable();

        if (!request.IncludeInactive)
        {
            categoriesQuery = categoriesQuery.Where(c => c.IsActive);
        }

        if (request.ParentId.HasValue)
        {
            // Get subtree starting from specific category
            categoriesQuery = categoriesQuery.Where(c => c.Id == request.ParentId.Value || c.Path!.StartsWith($"/{request.ParentId.Value}/"));
        }

        var categories = await categoriesQuery
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        // Build hierarchical tree
        var rootCategories = categories.Where(c => c.ParentId == null).ToList();
        var categoryTree = new List<CategoryTreeNode>();

        foreach (var rootCategory in rootCategories)
        {
            var treeNode = BuildCategoryTree(rootCategory, categories, request.MaxDepth ?? int.MaxValue, 0);
            if (treeNode != null)
            {
                categoryTree.Add(treeNode);
            }
        }

        return Result<CategoryTreeResponse>.Success(
            new CategoryTreeResponse
            {
                Categories = categoryTree.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToList()
            }
        );
    }

    private CategoryTreeNode? BuildCategoryTree(Domain.Entities.Category category, List<Domain.Entities.Category> allCategories, int maxDepth, int currentDepth)
    {
        if (currentDepth >= maxDepth)
        {
            return null;
        }

        var node = new CategoryTreeNode
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            Icon = category.Icon,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            IsFeatured = category.IsFeatured,
            Path = category.Path,
            Level = category.Level,
            ProductCount = category.ProductCount
        };

        // Get children and recursively build tree
        var children = allCategories.Where(c => c.ParentId == category.Id).ToList();

        foreach (var child in children.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name))
        {
            var childNode = BuildCategoryTree(child, allCategories, maxDepth, currentDepth + 1);
            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }
}