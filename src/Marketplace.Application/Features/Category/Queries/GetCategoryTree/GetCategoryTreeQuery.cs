using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Category.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Category.Queries.GetCategoryTree;

public class GetCategoryTreeQuery : IRequest<Result<CategoryTreeResponse>>
{
    public bool IncludeInactive { get; set; } = false;
    public bool IncludeProductCount { get; set; } = true;
    public int? MaxDepth { get; set; } // Limit tree depth, null = unlimited
    public Guid? ParentId { get; set; } // Get subtree starting from specific category, null = full tree
}

public class CategoryTreeResponse
{
    public List<CategoryTreeNode> Categories { get; set; } = new();
}

public class CategoryTreeNode
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public string? Path { get; set; }
    public int Level { get; set; }
    public int ProductCount { get; set; }
    public List<CategoryTreeNode> Children { get; set; } = new();
}