using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlazorServerSideClient.Models;

public class NullEmasilSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}