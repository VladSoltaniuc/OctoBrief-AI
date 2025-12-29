using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IAiService
{
  Task<AiSummaryResult> SummarizeContentAsync(string websiteName, List<string> headlines, string content, List<string>? sourceUrls = null, string? topic = null);
  Task<AiSummaryResult> SummarizeMultiSourceNewsAsync(string topic, string country, List<WebsiteNewsData> websiteData);
}

public record AiSummaryResult(
  bool Success,
  string Subject,
  string HtmlContent,
  string? Error = null
);
