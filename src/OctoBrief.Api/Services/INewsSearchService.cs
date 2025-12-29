namespace OctoBrief.Api.Services;

public interface INewsSearchService
{
  Task<NewsSearchResult> SearchNewsSourcesAsync(string topic, string country);
}

public record NewsSearchResult(
    bool Success,
    List<NewsSource> Sources,
    string? Error = null
);

public record NewsSource(
    string Name,
    string Url,
    bool IsMajorOutlet,
    string Country
);
