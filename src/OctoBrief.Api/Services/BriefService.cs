using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

/// <summary>
/// Service responsible for orchestrating brief generation from multiple news sources.
/// Handles the workflow of searching, scraping, and summarizing news.
/// </summary>
public class BriefService : IBriefService
{
  private readonly IScraperService _scraperService;
  private readonly IAiService _aiService;
  private readonly INewsSearchService _newsSearchService;
  private readonly ILogger<BriefService> _logger;

  private const int MaxSourcesToScrape = 5;
  private const int MaxHeadlinesPerSource = 10;

  public BriefService(
    IScraperService scraperService,
    IAiService aiService,
    INewsSearchService newsSearchService,
    ILogger<BriefService> logger)
  {
    _scraperService = scraperService;
    _aiService = aiService;
    _newsSearchService = newsSearchService;
    _logger = logger;
  }

  public async Task<BriefGenerationResult> GenerateBriefAsync(string topic, string country)
  {
    _logger.LogInformation("Generating brief for Topic: {Topic}, Country: {Country}", topic, country);

    // Step 1: Search for relevant news sources
    var searchResult = await _newsSearchService.SearchNewsSourcesAsync(topic, country);
    if (!searchResult.Success || searchResult.Sources.Count == 0)
    {
      return CreateFailureResult("Failed to find news sources for the topic");
    }

    // Step 2: Scrape each news source
    var (websiteData, results) = await ScrapeNewsSourcesAsync(searchResult.Sources);

    if (websiteData.Count == 0)
    {
      return new BriefGenerationResult(
        Success: false,
        Subject: null,
        HtmlContent: null,
        WebsiteResults: results,
        TotalHeadlines: 0,
        ErrorMessage: "Failed to scrape any content from news sources"
      );
    }

    // Step 3: Generate AI summary
    var aiResult = await _aiService.SummarizeMultiSourceNewsAsync(topic, country, websiteData);

    if (!aiResult.Success)
    {
      return new BriefGenerationResult(
        Success: false,
        Subject: null,
        HtmlContent: null,
        WebsiteResults: results,
        TotalHeadlines: websiteData.Sum(w => w.Headlines.Count),
        ErrorMessage: aiResult.Error ?? "Failed to generate summary"
      );
    }

    return new BriefGenerationResult(
      Success: true,
      Subject: aiResult.Subject,
      HtmlContent: aiResult.HtmlContent,
      WebsiteResults: results,
      TotalHeadlines: websiteData.Sum(w => w.Headlines.Count)
    );
  }

  private async Task<(List<WebsiteNewsData> Data, List<WebsiteBriefResult> Results)> ScrapeNewsSourcesAsync(
    List<NewsSource> sources)
  {
    var websiteData = new List<WebsiteNewsData>();
    var results = new List<WebsiteBriefResult>();

    foreach (var source in sources.Take(MaxSourcesToScrape))
    {
      var (data, result) = await ScrapeSourceAsync(source);

      if (data != null)
      {
        websiteData.Add(data);
      }
      results.Add(result);
    }

    return (websiteData, results);
  }

  private async Task<(WebsiteNewsData? Data, WebsiteBriefResult Result)> ScrapeSourceAsync(NewsSource source)
  {
    try
    {
      var scrapeResult = await _scraperService.ScrapeWebsiteAsync(source.Url);

      if (scrapeResult.Success && scrapeResult.Headlines.Count > 0)
      {
        var data = new WebsiteNewsData(
          source.Name,
          source.Url,
          source.IsMajorOutlet,
          scrapeResult.Headlines.Take(MaxHeadlinesPerSource).ToList(),
          scrapeResult.Content
        );

        var result = new WebsiteBriefResult(
          source.Url,
          source.Name,
          true,
          scrapeResult.Headlines.Count,
          source.IsMajorOutlet
        );

        return (data, result);
      }

      return (null, new WebsiteBriefResult(
        source.Url,
        source.Name,
        false,
        0,
        source.IsMajorOutlet,
        scrapeResult.Error
      ));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error scraping {Source}", source.Name);
      return (null, new WebsiteBriefResult(
        source.Url,
        source.Name,
        false,
        0,
        source.IsMajorOutlet,
        ex.Message
      ));
    }
  }

  private static BriefGenerationResult CreateFailureResult(string errorMessage)
  {
    return new BriefGenerationResult(
      Success: false,
      Subject: null,
      HtmlContent: null,
      WebsiteResults: [],
      TotalHeadlines: 0,
      ErrorMessage: errorMessage
    );
  }
}
