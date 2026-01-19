using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Address> Addresses { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
