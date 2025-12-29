using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OctoBrief.Api.Services;

public class EmailService : IEmailService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<EmailService> _logger;

  public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
  {
    _configuration = configuration;
    _logger = logger;
  }

  public async Task<EmailResult> SendEmailAsync(string to, string subject, string content)
  {
    var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
    var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
    var senderEmail = _configuration["Email:SenderEmail"];
    var senderPassword = _configuration["Email:SenderPassword"];
    var senderName = _configuration["Email:SenderName"] ?? "OctoBrief";

    if (string.IsNullOrEmpty(senderEmail) || senderEmail == "your-email@gmail.com")
    {
      return new EmailResult(false, "Email not configured. Please set Email:SenderEmail and Email:SenderPassword in appsettings.json", "NotConfigured");
    }

    if (string.IsNullOrEmpty(senderPassword) || senderPassword == "your-app-password")
    {
      return new EmailResult(false, "Email password not configured. Please set Email:SenderPassword in appsettings.json", "NotConfigured");
    }

    try
    {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(senderName, senderEmail));
      message.To.Add(new MailboxAddress(to, to));
      message.Subject = subject;
      message.Body = new TextPart("html") { Text = content };

      using var client = new SmtpClient();
      await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
      await client.AuthenticateAsync(senderEmail, senderPassword);
      await client.SendAsync(message);
      await client.DisconnectAsync(true);

      _logger.LogInformation("Email sent successfully to {To}", to);
      return new EmailResult(true, $"Email sent successfully to {to}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email to {To}", to);
      return new EmailResult(false, ex.Message, ex.GetType().Name);
    }
  }
}
