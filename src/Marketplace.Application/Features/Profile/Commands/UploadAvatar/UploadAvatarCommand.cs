using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.UploadAvatar;

public class UploadAvatarCommand : IRequest<Result<UploadAvatarResponse>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class UploadAvatarResponse
{
    public string AvatarUrl { get; set; } = string.Empty;
}
