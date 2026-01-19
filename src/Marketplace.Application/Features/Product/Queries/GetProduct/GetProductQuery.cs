using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetProduct;

public class GetProductQuery : IRequest<Result<ProductDto>>
{
    public Guid ProductId { get; set; }
}
