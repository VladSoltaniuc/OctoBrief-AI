namespace OctoBrief.Api.Services;

public interface IScraperService
{
  Task<ScrapeResult> ScrapeWebsiteAsync(string url);
}

public record ScrapeResult(
  bool Success,
  string? Title,
  List<HeadlineWithUrl> Headlines,
  string Content,
  string? Error = null
);

public record HeadlineWithUrl(string Title, string Url);
