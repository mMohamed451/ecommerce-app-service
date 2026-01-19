using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "Admin", "Vendor", "Customer" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }
    }

    public static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        const string adminEmail = "admin@marketplace.com";
        const string adminPassword = "Admin@123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                IsEmailVerified = true,
                EmailVerifiedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                var adminRole = await roleManager.FindByNameAsync(UserRole.Admin.ToString());
                if (adminRole != null)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole.Name!);
                }
            }
        }
    }

    public static async Task SeedCategoriesAsync(IApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Slug = "electronics",
                Description = "Electronic devices and gadgets",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Clothing",
                Slug = "clothing",
                Description = "Fashion and apparel",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Home & Garden",
                Slug = "home-garden",
                Description = "Home improvement and gardening supplies",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Sports & Outdoors",
                Slug = "sports-outdoors",
                Description = "Sports equipment and outdoor gear",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Books",
                Slug = "books",
                Description = "Books and publications",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync(default);
    }

    public static async Task SeedProductsAsync(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Products.AnyAsync())
            return;

        // Get the first vendor user (if exists) or create one
        var vendorUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email!.Contains("vendor"));
        if (vendorUser == null)
        {
            // Create a test vendor
            vendorUser = new ApplicationUser
            {
                UserName = "vendor@test.com",
                Email = "vendor@test.com",
                FirstName = "Test",
                LastName = "Vendor",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(vendorUser, "Vendor@123!");
            await userManager.AddToRoleAsync(vendorUser, "Vendor");
        }

        var vendor = await context.Vendors.FirstOrDefaultAsync(v => v.UserId == vendorUser.Id);
        if (vendor == null)
        {
            vendor = new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = vendorUser.Id,
                BusinessName = "Test Vendor Store",
                BusinessEmail = vendorUser.Email!,
                BusinessPhone = "+1234567890",
                BusinessDescription = "A test vendor for demonstration purposes",
                VerificationStatus = VerificationStatus.Approved,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Vendors.AddAsync(vendor);
            await context.SaveChangesAsync(default);
        }

        var categories = await context.Categories.ToListAsync();

        var products = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendor.Id,
                CategoryId = categories.First(c => c.Name == "Electronics").Id,
                Name = "Wireless Bluetooth Headphones",
                Slug = "wireless-bluetooth-headphones",
                Description = "High-quality wireless headphones with noise cancellation",
                ShortDescription = "Premium wireless headphones",
                Price = 99.99m,
                CompareAtPrice = 129.99m,
                CostPrice = 50.00m,
                SKU = "WH-001",
                Barcode = "123456789012",
                TrackInventory = true,
                StockQuantity = 50,
                LowStockThreshold = 10,
                Weight = 0.3m,
                Status = ProductStatus.Published,
                IsActive = true,
                IsFeatured = true,
                MetaTitle = "Wireless Bluetooth Headphones - Premium Quality",
                MetaDescription = "Buy high-quality wireless headphones with noise cancellation",
                MetaKeywords = "headphones,wireless,bluetooth,audio",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendor.Id,
                CategoryId = categories.First(c => c.Name == "Electronics").Id,
                Name = "Smartphone Case",
                Slug = "smartphone-case",
                Description = "Protective case for smartphones with screen protector",
                ShortDescription = "Durable smartphone protection",
                Price = 19.99m,
                SKU = "SC-002",
                TrackInventory = true,
                StockQuantity = 100,
                Weight = 0.1m,
                Status = ProductStatus.Published,
                IsActive = true,
                MetaKeywords = "case,protection,smartphone",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendor.Id,
                CategoryId = categories.First(c => c.Name == "Clothing").Id,
                Name = "Cotton T-Shirt",
                Slug = "cotton-t-shirt",
                Description = "Comfortable 100% cotton t-shirt in various colors",
                ShortDescription = "Premium cotton t-shirt",
                Price = 15.99m,
                CompareAtPrice = 24.99m,
                SKU = "TS-003",
                TrackInventory = true,
                StockQuantity = 200,
                Weight = 0.2m,
                Status = ProductStatus.Published,
                IsActive = true,
                MetaKeywords = "t-shirt,cotton,clothing,casual",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendor.Id,
                CategoryId = categories.First(c => c.Name == "Home & Garden").Id,
                Name = "Garden Hose",
                Slug = "garden-hose",
                Description = "Durable 50ft garden hose with adjustable nozzle",
                ShortDescription = "Heavy-duty garden hose",
                Price = 34.99m,
                SKU = "GH-004",
                TrackInventory = true,
                StockQuantity = 30,
                Weight = 2.5m,
                Status = ProductStatus.Published,
                IsActive = true,
                MetaKeywords = "garden,hose,watering,outdoor",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendor.Id,
                CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id,
                Name = "Yoga Mat",
                Slug = "yoga-mat",
                Description = "Non-slip yoga mat for all fitness activities",
                ShortDescription = "Premium non-slip yoga mat",
                Price = 29.99m,
                SKU = "YM-005",
                TrackInventory = true,
                StockQuantity = 75,
                Weight = 1.2m,
                Status = ProductStatus.Published,
                IsActive = true,
                IsFeatured = true,
                MetaKeywords = "yoga,fitness,mat,exercise",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync(default);
    }
}
