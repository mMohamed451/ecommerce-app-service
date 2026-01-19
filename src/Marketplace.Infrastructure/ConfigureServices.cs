using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false; // Set to true when email verification is implemented
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Add SignInManager
        services.AddScoped<SignInManager<ApplicationUser>>();

        // Add Token Service
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        
        // Add Current User Service
        services.AddScoped<Application.Common.Interfaces.ICurrentUserService, CurrentUserService>();
        
        // Add Email Service (placeholder - implement with actual email provider)
        services.AddScoped<Application.Common.Interfaces.IEmailService, EmailService>();
        
        // Add File Storage Service
        services.AddScoped<Application.Common.Interfaces.IFileStorageService, FileStorageService>();
        
        // Add Search Index Service (placeholder - implement with Elasticsearch/Azure Cognitive Search)
        services.AddScoped<Application.Common.Interfaces.ISearchIndexService, SearchIndexService>();

        return services;
    }
}
