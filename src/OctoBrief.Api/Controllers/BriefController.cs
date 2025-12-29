using Microsoft.AspNetCore.Mvc;
using OctoBrief.Api.Models;
using OctoBrief.Api.Services;

namespace OctoBrief.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BriefController : ControllerBase
{
  private readonly IBriefService _briefService;
  private readonly IEmailService _emailService;

  public BriefController(
    IBriefService briefService,
    IEmailService emailService)
  {
    _briefService = briefService;
    _emailService = emailService;
  }

  [HttpPost("preview")]
  public async Task<IActionResult> Preview([FromBody] GenerateBriefRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Topic)) return BadRequest(new { success = false, message = "Topic is required" });
    var country = NormalizeCountry(request.Country);
    var briefResult = await _briefService.GenerateBriefAsync(request.Topic, country);
    if (!briefResult.Success) return HandleBriefError(briefResult);
    return Ok(new
    {
      success = true,
      subject = briefResult.Subject,
      htmlContent = briefResult.HtmlContent,
      websiteResults = briefResult.WebsiteResults,
      totalHeadlines = briefResult.TotalHeadlines,
      topic = request.Topic,
      country
    });
  }

  [HttpPost("send-email")]
  public async Task<IActionResult> SendEmail([FromBody] SendBriefEmailRequest request)
  {
    var requestId = Guid.NewGuid().ToString()[..8];
    if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest(new { success = false, message = "Email is required" });
    if (string.IsNullOrWhiteSpace(request.HtmlContent)) return BadRequest(new { success = false, message = "Content is required" });

    var subject = string.IsNullOrWhiteSpace(request.Subject) ? "Your News Brief from OctoBrief" : request.Subject;
    var emailResult = await _emailService.SendEmailAsync(request.Email, subject, request.HtmlContent);
    if (!emailResult.Success) return StatusCode(500, new { success = false, message = emailResult.Message ?? "Failed to send email" });

    return Ok(new { success = true, message = $"Brief sent successfully to {request.Email}" });
  }

  private static string NormalizeCountry(string? country)
  {
    return string.IsNullOrWhiteSpace(country) ? "global" : country;
  }

  private IActionResult HandleBriefError(BriefGenerationResult result)
  {
    if (result.WebsiteResults.Count == 0) return BadRequest(new { success = false, message = result.ErrorMessage });
    return BadRequest(new
    {
      success = false,
      message = result.ErrorMessage,
      results = result.WebsiteResults
    });
  }
}
