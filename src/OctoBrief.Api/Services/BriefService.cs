using OctoBrief.Api.Models;
using OctoBrief.Api.Security;

namespace OctoBrief.Api.Services;

public class BriefService : IBriefService
{
  private readonly IScraperService _scraperService;
  private readonly IAiService _aiService;

  public BriefService(IScraperService scraperService, IAiService aiService)
  {
    _scraperService = scraperService;
    _aiService = aiService;
  }

  public async Task<BriefDto> GetBriefAsync(string topic, string country)
  {
    List<NewsSource> sources = VerifiedLinks.CountryNewsSources[country.ToLowerInvariant().Trim()][topic.ToLowerInvariant().Trim()];
    var (websiteData, sourcesUsed) = await ScrapeNewsAsync(sources);
    if (websiteData.Count == 0) return new BriefDto { Success = false, Sources = 0, HtmlContent = string.Empty };
    return new BriefDto
    {
      Success = true,
      Sources = sourcesUsed,
      HtmlContent = await _aiService.GenerateCards(websiteData)
    };
  }

  private async Task<(List<WebsiteData> WebsiteData, int SourcesUsed)> ScrapeNewsAsync(List<NewsSource> sources)
  {
    var websiteData = new List<WebsiteData>();
    var sourcesUsed = 0;
    foreach (var source in sources.Take(5))
    {
      var headlines = await _scraperService.ScrapeWebsiteAsync(source.Url);
      if (headlines.Count == 0) continue;
      websiteData.Add(new WebsiteData(source.Name, source.Url, headlines.Take(10).ToList()));
      sourcesUsed++;
    }
    return (websiteData, sourcesUsed);
  }
}
