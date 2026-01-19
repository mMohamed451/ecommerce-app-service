using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.UploadProductImage;

public class UploadProductImageCommand : IRequest<Result<ProductImageResponse>>
{
    public Guid ProductId { get; set; }
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
    public string? AltText { get; set; }
}

public class ProductImageResponse
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
