using OctoBrief.Api.Services;

namespace OctoBrief.Api.Models;

#region Brief DTOs
public record GenerateBriefRequest
{
  public string Topic { get; init; } = "technology";
  public string Country { get; init; } = "romania";
}
public record SendBriefEmailRequest
{
  public string Email { get; init; } = string.Empty;
  public string Subject { get; init; } = string.Empty;
  public string HtmlContent { get; init; } = string.Empty;
}
public record BriefDto
{
  public bool Success { get; init; }
  public int Sources { get; init; } = 0;
  public string HtmlContent { get; init; } = string.Empty;
}
#endregion


#region Email DTOs
public record EmailDto
{
  public bool Success { get; init; }
  public string Message { get; init; } = string.Empty;
}
#endregion


#region Scraper DTOs
public record NewsSource(
    string Name,
    string Url
);
public record WebsiteData(
  string Name,
  string Url,
  List<HeadlineWithUrl> Headlines
);
#endregion