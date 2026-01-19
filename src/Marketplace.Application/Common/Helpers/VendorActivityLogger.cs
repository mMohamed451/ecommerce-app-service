using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Common.Helpers;

public class VendorActivityLogger
{
    private readonly IApplicationDbContext _context;

    public VendorActivityLogger(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActivityAsync(
        Guid vendorId,
        ActivityType activityType,
        string description,
        string? entityType = null,
        Guid? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? performedBy = null)
    {
        var log = new VendorActivityLog
        {
            VendorId = vendorId,
            ActivityType = activityType,
            Description = description,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            PerformedBy = performedBy
        };

        _context.VendorActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task LogActivityAsync(
        Guid vendorId,
        ActivityType activityType,
        string description,
        object? oldEntity = null,
        object? newEntity = null,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? performedBy = null)
    {
        string? oldValues = oldEntity != null ? System.Text.Json.JsonSerializer.Serialize(oldEntity) : null;
        string? newValues = newEntity != null ? System.Text.Json.JsonSerializer.Serialize(newEntity) : null;

        await LogActivityAsync(
            vendorId,
            activityType,
            description,
            oldEntity?.GetType().Name,
            null,
            oldValues,
            newValues,
            ipAddress,
            userAgent,
            performedBy);
    }
}
