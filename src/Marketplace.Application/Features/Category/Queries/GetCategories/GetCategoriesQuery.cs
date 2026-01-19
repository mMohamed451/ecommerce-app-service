using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Category.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Category.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
    public Guid? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    public bool IncludeChildren { get; set; } = true;
}
