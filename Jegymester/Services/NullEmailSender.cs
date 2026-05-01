using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

public class NullEmailSender : IEmailSender
{
    private readonly ILogger<NullEmailSender> _logger;

    public NullEmailSender(ILogger<NullEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Log the email instead of sending. Replace with real email sender in production.
        _logger.LogInformation("Email to {Email} subject {Subject}: {Message}", email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}