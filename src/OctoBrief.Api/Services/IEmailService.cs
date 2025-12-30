using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IEmailService
{
  Task<EmailDto> SendEmailAsync(string to, string subject, string content);
  EmailDto FakeSendEmail(string to);
}