using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.UploadCoverImage;

public class UploadCoverImageCommand : IRequest<Result<UploadCoverImageResponse>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class UploadCoverImageResponse
{
    public string CoverImageUrl { get; set; } = string.Empty;
}
