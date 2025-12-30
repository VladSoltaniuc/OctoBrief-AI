namespace OctoBrief.Api.Services;

public interface IScraperService
{
  Task<List<HeadlineWithUrl>> ScrapeWebsiteAsync(string url);
}

public record HeadlineWithUrl(string Title, string Url);
