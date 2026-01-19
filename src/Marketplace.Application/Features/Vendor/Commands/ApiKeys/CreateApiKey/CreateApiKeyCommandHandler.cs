using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Marketplace.Application.Features.Vendor.Commands.ApiKeys.CreateApiKey;

public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, Result<ApiKeyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateApiKeyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ApiKeyResponse>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ApiKeyResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ApiKeyResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // Generate API key
        var apiKey = GenerateApiKey();
        var hashedApiKey = HashApiKey(apiKey);

        var vendorApiKey = new VendorApiKey
        {
            VendorId = vendor.Id,
            KeyName = request.KeyName,
            ApiKey = hashedApiKey, // Store hashed version
            ExpiresAt = request.ExpiresAt,
            AllowedIps = request.AllowedIps,
            Permissions = request.Permissions ?? Array.Empty<string>(),
            IsActive = true,
            CreatedBy = userId.Value.ToString()
        };

        _context.VendorApiKeys.Add(vendorApiKey);
        await _context.SaveChangesAsync(cancellationToken);

        // Return the plain API key only once
        return Result<ApiKeyResponse>.Success(new ApiKeyResponse
        {
            Id = vendorApiKey.Id,
            KeyName = vendorApiKey.KeyName,
            ApiKey = apiKey, // Return plain key only on creation
            ExpiresAt = vendorApiKey.ExpiresAt,
            AllowedIps = vendorApiKey.AllowedIps,
            Permissions = vendorApiKey.Permissions
        });
    }

    private static string GenerateApiKey()
    {
        // Generate a secure random API key
        const string prefix = "mkp_";
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        var key = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
        return prefix + key;
    }

    private static string HashApiKey(string apiKey)
    {
        // Hash the API key for storage (use SHA256)
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hashBytes);
    }
}
