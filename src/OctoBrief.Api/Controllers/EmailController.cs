using Microsoft.AspNetCore.Mvc;
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
  public async Task<IActionResult> Send([FromBody] SendEmailRequest request)
  {
    var result = await _emailService.SendEmailAsync(request.To, request.Subject, request.Content);
    if (result.Success) return Ok(result);
    return BadRequest(result);
  }
}

public record SendEmailRequest(string To, string Subject, string Content);
