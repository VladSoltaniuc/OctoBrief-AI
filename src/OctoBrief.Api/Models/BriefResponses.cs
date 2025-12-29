using OctoBrief.Api.Services;

namespace OctoBrief.Api.Models;

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
