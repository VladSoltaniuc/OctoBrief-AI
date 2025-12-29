using Microsoft.AspNetCore.Mvc;
using OctoBrief.Api.Services;
using System.Text;

namespace OctoBrief.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BriefController : ControllerBase
{
  private readonly IScraperService _scraperService;
  private readonly IAiService _aiService;
  private readonly IEmailService _emailService;
  private readonly INewsSearchService _newsSearchService;
  private readonly ILogger<BriefController> _logger;

  public BriefController(
    IScraperService scraperService,
    IAiService aiService,
    IEmailService emailService,
    INewsSearchService newsSearchService,
    ILogger<BriefController> logger)
  {
    _scraperService = scraperService;
    _aiService = aiService;
    _emailService = emailService;
    _newsSearchService = newsSearchService;
    _logger = logger;
  }

  /// <summary>
  /// Generate and send a news brief from the provided websites to the specified email
  /// </summary>
  [HttpPost("generate")]
  public async Task<IActionResult> Generate([FromBody] GenerateBriefRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
    {
      return BadRequest(new { success = false, message = "Email is required" });
    }

    if (string.IsNullOrWhiteSpace(request.Topic))
    {
      return BadRequest(new { success = false, message = "Topic is required" });
    }

    var country = string.IsNullOrWhiteSpace(request.Country) ? "global" : request.Country;

    _logger.LogInformation("Generating brief for {Email} - Topic: {Topic}, Country: {Country}",
      request.Email, request.Topic, country);

    // Step 1: Search for relevant news sources
    var searchResult = await _newsSearchService.SearchNewsSourcesAsync(request.Topic, country);
    if (!searchResult.Success || searchResult.Sources.Count == 0)
    {
      return BadRequest(new { success = false, message = "Failed to find news sources for the topic" });
    }

    var results = new List<WebsiteBriefResult>();
    var websiteData = new List<WebsiteNewsData>();

    // Step 2: Scrape each news source (max 5)
    foreach (var source in searchResult.Sources.Take(5))
    {
      try
      {
        var scrapeResult = await _scraperService.ScrapeWebsiteAsync(source.Url);

        if (scrapeResult.Success && scrapeResult.Headlines.Count > 0)
        {
          websiteData.Add(new WebsiteNewsData(
            source.Name,
            source.Url,
            source.IsMajorOutlet,
            scrapeResult.Headlines.Take(10).ToList(), // Get more headlines for AI to choose from
            scrapeResult.Content
          ));

          results.Add(new WebsiteBriefResult(source.Url, source.Name, true, scrapeResult.Headlines.Count, source.IsMajorOutlet));
        }
        else
        {
          results.Add(new WebsiteBriefResult(source.Url, source.Name, false, 0, source.IsMajorOutlet, scrapeResult.Error));
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error scraping {Source}", source.Name);
        results.Add(new WebsiteBriefResult(source.Url, source.Name, false, 0, source.IsMajorOutlet, ex.Message));
      }
    }

    if (websiteData.Count == 0)
    {
      return BadRequest(new
      {
        success = false,
        message = "Failed to scrape any content from news sources",
        results
      });
    }

    // Step 3: Generate AI summary (max 2 headlines per site)
    var aiResult = await _aiService.SummarizeMultiSourceNewsAsync(request.Topic, country, websiteData);

    if (!aiResult.Success)
    {
      return StatusCode(500, new
      {
        success = false,
        message = "Failed to generate summary",
        error = aiResult.Error,
        results
      });
    }

    // Step 4: Send email
    var emailResult = await _emailService.SendEmailAsync(
      request.Email,
      aiResult.Subject,
      aiResult.HtmlContent
    );

    if (!emailResult.Success)
    {
      return StatusCode(500, new
      {
        success = false,
        message = "Failed to send email",
        error = emailResult.Message,
        results,
        subject = aiResult.Subject
      });
    }

    return Ok(new GenerateBriefResponse
    {
      Success = true,
      Message = $"Brief sent successfully to {request.Email}",
      Subject = aiResult.Subject,
      WebsiteResults = results,
      TotalHeadlines = websiteData.Sum(w => w.Headlines.Count),
      Topic = request.Topic,
      Country = country
    });
  }

  /// <summary>
  /// Preview the brief without sending an email (for testing)
  /// </summary>
  [HttpPost("preview")]
  public async Task<IActionResult> Preview([FromBody] GenerateBriefRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Topic))
    {
      return BadRequest(new { success = false, message = "Topic is required" });
    }

    var country = string.IsNullOrWhiteSpace(request.Country) ? "global" : request.Country;

    // Search for news sources
    var searchResult = await _newsSearchService.SearchNewsSourcesAsync(request.Topic, country);
    if (!searchResult.Success || searchResult.Sources.Count == 0)
    {
      return BadRequest(new { success = false, message = "Failed to find news sources" });
    }

    var results = new List<WebsiteBriefResult>();
    var websiteData = new List<WebsiteNewsData>();

    foreach (var source in searchResult.Sources.Take(5))
    {
      var scrapeResult = await _scraperService.ScrapeWebsiteAsync(source.Url);

      if (scrapeResult.Success && scrapeResult.Headlines.Count > 0)
      {
        websiteData.Add(new WebsiteNewsData(
          source.Name,
          source.Url,
          source.IsMajorOutlet,
          scrapeResult.Headlines.Take(10).ToList(),
          scrapeResult.Content
        ));

        results.Add(new WebsiteBriefResult(source.Url, source.Name, true, scrapeResult.Headlines.Count, source.IsMajorOutlet));
      }
      else
      {
        results.Add(new WebsiteBriefResult(source.Url, source.Name, false, 0, source.IsMajorOutlet, scrapeResult.Error));
      }
    }

    if (websiteData.Count == 0)
    {
      return BadRequest(new { success = false, message = "Failed to scrape any content" });
    }

    var aiResult = await _aiService.SummarizeMultiSourceNewsAsync(request.Topic, country, websiteData);

    return Ok(new
    {
      success = aiResult.Success,
      subject = aiResult.Subject,
      htmlContent = aiResult.HtmlContent,
      websiteResults = results,
      totalHeadlines = websiteData.Sum(w => w.Headlines.Count),
      topic = request.Topic,
      country
    });
  }

  /// <summary>
  /// Send an already-generated brief to email
  /// </summary>
  [HttpPost("send-email")]
  public async Task<IActionResult> SendEmail([FromBody] SendBriefEmailRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
    {
      return BadRequest(new { success = false, message = "Email is required" });
    }

    if (string.IsNullOrWhiteSpace(request.HtmlContent))
    {
      return BadRequest(new { success = false, message = "Content is required" });
    }

    var subject = string.IsNullOrWhiteSpace(request.Subject) ? "Your News Brief from OctoBrief" : request.Subject;

    var emailResult = await _emailService.SendEmailAsync(request.Email, subject, request.HtmlContent);

    if (!emailResult.Success)
    {
      return StatusCode(500, new { success = false, message = emailResult.Message ?? "Failed to send email" });
    }

    return Ok(new { success = true, message = $"Brief sent successfully to {request.Email}" });
  }

  private string ExtractWebsiteName(string url)
  {
    try
    {
      var uri = new Uri(url);
      var host = uri.Host.Replace("www.", "");
      var parts = host.Split('.');
      return char.ToUpper(parts[0][0]) + parts[0][1..];
    }
    catch
    {
      return "Website";
    }
  }
}

public record GenerateBriefRequest
{
  public string Email { get; init; } = string.Empty;
  public string Topic { get; init; } = string.Empty;
  public string? Country { get; init; } = "global";
}

public record WebsiteNewsData(
  string Name,
  string Url,
  bool IsMajorOutlet,
  List<HeadlineWithUrl> Headlines,
  string Content
);

public record WebsiteBriefResult(
  string Url,
  string WebsiteName,
  bool Success,
  int HeadlinesFound,
  bool IsMajorOutlet = false,
  string? Error = null
);

public record GenerateBriefResponse
{
  public bool Success { get; init; }
  public string Message { get; init; } = string.Empty;
  public string Subject { get; init; } = string.Empty;
  public List<WebsiteBriefResult> WebsiteResults { get; init; } = [];
  public int TotalHeadlines { get; init; }
  public string Topic { get; init; } = string.Empty;
  public string Country { get; init; } = string.Empty;
}

public record SendBriefEmailRequest
{
  public string Email { get; init; } = string.Empty;
  public string Subject { get; init; } = string.Empty;
  public string HtmlContent { get; init; } = string.Empty;
}
