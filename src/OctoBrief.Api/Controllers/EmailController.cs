using Microsoft.AspNetCore.Mvc;
using OctoBrief.Api.Models;
using OctoBrief.Api.Security;
using OctoBrief.Api.Services;

namespace OctoBrief.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
  private readonly IEmailService _emailService;

  public EmailController(IEmailService emailService)
  {
    _emailService = emailService;
  }

  [HttpPost("send")]
  public async Task<IActionResult> SendEmail([FromBody] SendBriefEmailRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest();
    if (string.IsNullOrWhiteSpace(request.HtmlContent)) return BadRequest();
    if (HtmlSecurity.MaliciousContent(request.HtmlContent))
      return Ok(_emailService.FakeSendEmail(request.Email));

    var subject = string.IsNullOrWhiteSpace(request.Subject) ? "Your News Email from OctoEmail" : request.Subject;
    EmailDto mail = await _emailService.SendEmailAsync(request.Email, subject, request.HtmlContent);
    if (!mail.Success) return StatusCode(500);
    return Ok(mail);
  }
}
