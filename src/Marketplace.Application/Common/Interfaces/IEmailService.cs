namespace Marketplace.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetLink);
    Task SendEmailVerificationEmailAsync(string email, string verificationLink);
}
