using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.ApiKeys.CreateApiKey;

public class CreateApiKeyCommand : IRequest<Result<ApiKeyResponse>>
{
    public string KeyName { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string[]? AllowedIps { get; set; }
    public string[]? Permissions { get; set; }
}

public class ApiKeyResponse
{
    public Guid Id { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty; // Only shown once on creation
    public DateTime? ExpiresAt { get; set; }
    public string[]? AllowedIps { get; set; }
    public string[]? Permissions { get; set; }
}
