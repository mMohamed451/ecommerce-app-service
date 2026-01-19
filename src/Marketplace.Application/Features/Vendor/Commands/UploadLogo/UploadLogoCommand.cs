using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.UploadLogo;

public class UploadLogoCommand : IRequest<Result<UploadLogoResponse>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class UploadLogoResponse
{
    public string LogoUrl { get; set; } = string.Empty;
}
