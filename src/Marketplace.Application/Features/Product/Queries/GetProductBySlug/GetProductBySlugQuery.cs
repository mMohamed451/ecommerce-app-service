using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetProductBySlug;

public class GetProductBySlugQuery : IRequest<Result<ProductDto>>
{
    public string Slug { get; set; } = string.Empty;
}
