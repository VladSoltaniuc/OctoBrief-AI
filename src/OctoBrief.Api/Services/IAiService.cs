using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IAiService
{
  Task<AiSummaryResult> SummarizeMultiSourceNewsAsync(string topic, string country, List<WebsiteNewsData> websiteData);
}

public record AiSummaryResult(
  bool Success,
  string Subject,
  string HtmlContent,
  string? Error = null
);
