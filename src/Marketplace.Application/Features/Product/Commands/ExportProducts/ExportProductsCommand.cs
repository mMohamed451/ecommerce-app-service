using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.ExportProducts;

public class ExportProductsCommand : IRequest<Result<ExportProductsResponse>>
{
    public Guid? CategoryId { get; set; }
    public Domain.Entities.ProductStatus? Status { get; set; }
    public bool? IsActive { get; set; }
}

public class ExportProductsResponse
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/csv";
}
