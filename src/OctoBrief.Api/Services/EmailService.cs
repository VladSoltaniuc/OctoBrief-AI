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
    var requestId = Guid.NewGuid().ToString()[..8];
    _logger.LogInformation("[EmailService-{RequestId}] SendEmailAsync called for {To}", requestId, to);

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
      _logger.LogInformation("[EmailService-{RequestId}] Creating message for {To}", requestId, to);
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(senderName, senderEmail));
      message.To.Add(MailboxAddress.Parse(to));
      message.Subject = subject;
      message.Body = new TextPart("html") { Text = content };

      _logger.LogInformation("[EmailService-{RequestId}] Connecting to SMTP server {Host}:{Port}", requestId, smtpHost, smtpPort);
      using var client = new SmtpClient();
      await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

      _logger.LogInformation("[EmailService-{RequestId}] Authenticating", requestId);
      await client.AuthenticateAsync(senderEmail, senderPassword);

      _logger.LogInformation("[EmailService-{RequestId}] Sending email NOW", requestId);
      await client.SendAsync(message);
      _logger.LogInformation("[EmailService-{RequestId}] Email SENT via SMTP", requestId);

      await client.DisconnectAsync(true);

      _logger.LogInformation("[EmailService-{RequestId}] Email sent successfully to {To}", requestId, to);
      return new EmailResult(true, $"Email sent successfully to {to}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "[EmailService-{RequestId}] Failed to send email to {To}", requestId, to);
      return new EmailResult(false, ex.Message, ex.GetType().Name);
    }
  }
}
