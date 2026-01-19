using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; set; }
}
