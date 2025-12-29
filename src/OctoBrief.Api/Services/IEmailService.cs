namespace OctoBrief.Api.Services;

public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(string to, string subject, string content);
}

public record EmailResult(bool Success, string Message, string? Error = null);
