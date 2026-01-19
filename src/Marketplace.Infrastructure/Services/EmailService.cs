using Marketplace.Application.Common.Interfaces;

namespace Marketplace.Infrastructure.Services;

public class EmailService : IEmailService
{
    // TODO: Implement email sending using SendGrid, SMTP, or other email service
    public Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        // Placeholder implementation
        // In production, integrate with an email service provider
        return Task.CompletedTask;
    }

    public Task SendEmailVerificationEmailAsync(string email, string verificationLink)
    {
        // Placeholder implementation
        // In production, integrate with an email service provider
        return Task.CompletedTask;
    }
}
