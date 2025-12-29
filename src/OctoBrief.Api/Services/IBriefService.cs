using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IBriefService
{
  Task<BriefGenerationResult> GenerateBriefAsync(string topic, string country);
}

public record BriefGenerationResult(
  bool Success,
  string? Subject,
  string? HtmlContent,
  List<WebsiteBriefResult> WebsiteResults,
  int TotalHeadlines,
  string? ErrorMessage = null
);
