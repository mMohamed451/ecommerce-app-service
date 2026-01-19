using System.Reflection;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Common;
using Marketplace.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorVerification> VendorVerifications => Set<VendorVerification>();
    public DbSet<VendorBankAccount> VendorBankAccounts => Set<VendorBankAccount>();
    public DbSet<VendorCommission> VendorCommissions => Set<VendorCommission>();
    public DbSet<VendorSubscription> VendorSubscriptions => Set<VendorSubscription>();
    public DbSet<VendorLocation> VendorLocations => Set<VendorLocation>();
    public DbSet<VendorBusinessHours> VendorBusinessHours => Set<VendorBusinessHours>();
    public DbSet<VendorShippingSettings> VendorShippingSettings => Set<VendorShippingSettings>();
    public DbSet<VendorTaxInfo> VendorTaxInfos => Set<VendorTaxInfo>();
    public DbSet<VendorApiKey> VendorApiKeys => Set<VendorApiKey>();
    public DbSet<VendorActivityLog> VendorActivityLogs => Set<VendorActivityLog>();
    public DbSet<VendorNotificationPreference> VendorNotificationPreferences => Set<VendorNotificationPreference>();
    public DbSet<VendorPerformanceMetrics> VendorPerformanceMetrics => Set<VendorPerformanceMetrics>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariation> ProductVariations => Set<ProductVariation>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductVariationAttribute> ProductVariationAttributes => Set<ProductVariationAttribute>();

    // Cart and Wishlist
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        // Update ApplicationUser UpdatedAt
        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Token });
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserProfile
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Address
        builder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure NotificationPreference
        builder.Entity<NotificationPreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(u => u.NotificationPreference)
                .HasForeignKey<NotificationPreference>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Vendor
        builder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.BusinessEmail);
            entity.HasOne(e => e.User)
                .WithOne(u => u.Vendor)
                .HasForeignKey<Vendor>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorVerification
        builder.Entity<VendorVerification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.VerificationDocuments)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorBankAccount
        builder.Entity<VendorBankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.BankAccounts)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorCommission
        builder.Entity<VendorCommission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.Commissions)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorSubscription
        builder.Entity<VendorSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.Subscriptions)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorLocation
        builder.Entity<VendorLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.Locations)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorBusinessHours
        builder.Entity<VendorBusinessHours>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.BusinessHours)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorShippingSettings
        builder.Entity<VendorShippingSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId).IsUnique();
            entity.HasOne(e => e.Vendor)
                .WithOne(v => v.ShippingSettings)
                .HasForeignKey<VendorShippingSettings>(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorTaxInfo
        builder.Entity<VendorTaxInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => e.TaxId);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.TaxInfos)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorApiKey
        builder.Entity<VendorApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => e.ApiKey);
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.ApiKeys)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorActivityLog
        builder.Entity<VendorActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => new { e.VendorId, e.CreatedAt });
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.ActivityLogs)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorNotificationPreference
        builder.Entity<VendorNotificationPreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId).IsUnique();
            entity.HasOne(e => e.Vendor)
                .WithOne(v => v.NotificationPreference)
                .HasForeignKey<VendorNotificationPreference>(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure VendorPerformanceMetrics
        builder.Entity<VendorPerformanceMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => new { e.VendorId, e.Period, e.PeriodStart });
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.PerformanceMetrics)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Product
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.SKU);
            entity.HasIndex(e => new { e.Status, e.IsActive });
            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.Products)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Category
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => new { e.IsActive, e.DisplayOrder });
            entity.HasOne(e => e.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ProductImage
        builder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.ProductId, e.DisplayOrder });
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductVariation
        builder.Entity<ProductVariation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.SKU);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Variations)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductAttribute
        builder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.ProductId, e.DisplayOrder });
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Attributes)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductVariationAttribute
        builder.Entity<ProductVariationAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductVariationId);
            entity.HasOne(e => e.ProductVariation)
                .WithMany(v => v.VariationAttributes)
                .HasForeignKey(e => e.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Cart
        builder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => new { e.UserId, e.IsActive });
            entity.HasIndex(e => new { e.SessionId, e.IsActive });
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure CartItem
        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CartId);
            entity.HasIndex(e => new { e.ProductId, e.ProductVariationId });
            entity.HasOne(e => e.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ProductVariation)
                .WithMany()
                .HasForeignKey(e => e.ProductVariationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Wishlist
        builder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => new { e.UserId, e.IsActive });
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure WishlistItem
        builder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.WishlistId);
            entity.HasIndex(e => new { e.WishlistId, e.ProductId }).IsUnique();
            entity.HasOne(e => e.Wishlist)
                .WithMany(w => w.Items)
                .HasForeignKey(e => e.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure entity mappings
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
