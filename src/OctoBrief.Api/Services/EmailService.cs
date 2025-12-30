using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public class EmailService : IEmailService
{
  private readonly IConfiguration _configuration;

  public EmailService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public async Task<EmailDto> SendEmailAsync(string to, string subject, string content)
  {
    int.TryParse(_configuration["Email:SmtpPort"], out int smtpPort);
    var smtpHost = _configuration["Email:SmtpHost"];
    var senderEmail = _configuration["Email:SenderEmail"];
    var senderPassword = _configuration["Email:SenderPassword"];
    var senderName = _configuration["Email:SenderName"];
    if (!IsSMTPConfigured()) return new EmailDto { Success = false, Message = "Email configuration is invalid" };

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(senderName, senderEmail));
    message.To.Add(MailboxAddress.Parse(to));
    message.Subject = subject;
    message.Body = new TextPart("html") { Text = content };

    using var client = new SmtpClient();
    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
    await client.AuthenticateAsync(senderEmail, senderPassword);
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
    return new EmailDto { Success = true, Message = $"Email sent successfully to {to}" };
  }

  public EmailDto FakeSendEmail(string to)
  {
    if (!IsSMTPConfigured()) return new EmailDto { Success = false, Message = "Email configuration is invalid" };
    return new EmailDto { Success = true, Message = $"Email sent successfully to {to}" };
  }

  private bool IsSMTPConfigured()
  {
    int.TryParse(_configuration["Email:SmtpPort"], out int smtpPort);
    return smtpPort > 0
    && !string.IsNullOrEmpty(_configuration["Email:SmtpHost"])
    && !string.IsNullOrEmpty(_configuration["Email:SenderEmail"])
    && !string.IsNullOrEmpty(_configuration["Email:SenderPassword"])
    && !string.IsNullOrEmpty(_configuration["Email:SenderName"]);
  }
}
