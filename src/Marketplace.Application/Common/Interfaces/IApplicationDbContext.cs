using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Address> Addresses { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<Vendor> Vendors { get; }
    DbSet<VendorVerification> VendorVerifications { get; }
    DbSet<VendorBankAccount> VendorBankAccounts { get; }
    DbSet<VendorCommission> VendorCommissions { get; }
    DbSet<VendorSubscription> VendorSubscriptions { get; }
    DbSet<VendorLocation> VendorLocations { get; }
    DbSet<VendorBusinessHours> VendorBusinessHours { get; }
    DbSet<VendorShippingSettings> VendorShippingSettings { get; }
    DbSet<VendorTaxInfo> VendorTaxInfos { get; }
    DbSet<VendorApiKey> VendorApiKeys { get; }
    DbSet<VendorActivityLog> VendorActivityLogs { get; }
    DbSet<VendorNotificationPreference> VendorNotificationPreferences { get; }
    DbSet<VendorPerformanceMetrics> VendorPerformanceMetrics { get; }
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductVariation> ProductVariations { get; }
    DbSet<ProductAttribute> ProductAttributes { get; }
    DbSet<ProductVariationAttribute> ProductVariationAttributes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
