namespace OctoBrief.Api.Models;

public record GenerateBriefRequest
{
  public string Email { get; init; } = string.Empty;
  public string Topic { get; init; } = string.Empty;
  public string? Country { get; init; } = "global";
}

public record SendBriefEmailRequest
{
  public string Email { get; init; } = string.Empty;
  public string Subject { get; init; } = string.Empty;
  public string HtmlContent { get; init; } = string.Empty;
}
