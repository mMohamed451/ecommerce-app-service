using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Result<bool>>
{
    public Guid CategoryId { get; set; }
}
