using OpenAI;
using OpenAI.Chat;
using OctoBrief.Api.Models;
using System.Text;

namespace OctoBrief.Api.Services;

public class AiService : IAiService
{
  private readonly IConfiguration _configuration;
  private readonly HashSet<string> _allowedDomains;

  public AiService(IConfiguration configuration, ILogger<AiService> logger)
  {
    _configuration = configuration;
    _allowedDomains = AllowedDomainsProvider.GetAllowedDomains();
  }

  public async Task<AiSummaryResult> SummarizeMultiSourceNewsAsync(string topic, string country, List<WebsiteNewsData> websiteData)
  {
    var apiKey = _configuration["OpenAI:ApiKey"];
    //TODO: Improve
    if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY")
      return await GenerateMultiSourceFallbackSummary(topic, country, websiteData);

    return await GenerateMultiSourceFallbackSummary(topic, country, websiteData);
  }

  private async Task<AiSummaryResult> GenerateMultiSourceFallbackSummary(string topic, string country, List<WebsiteNewsData> websiteData)
  {
    var sb = new StringBuilder();
    var countryDisplay = country.Equals("global", StringComparison.OrdinalIgnoreCase) ? "Global" : country;
    int validHeadlineCount = 0;
    foreach (var source in websiteData)
    {
      var validHeadlines = source.Headlines.Where(h => IsValidHeadline(h.Title) && IsValidArticleUrl(h.Url)).Take(2).ToList();
      foreach (var headline in validHeadlines)
      {
        var displayUrl = headline.Url;
        var uri = new Uri(headline.Url);
        displayUrl = uri.Host;

        sb.AppendLine("<div class=\"story\">");
        sb.AppendLine($"<h4>{SetStrLimit(headline.Title, 100)}</h4>");
        sb.AppendLine($"<div class=\"description\">{await GenerateDescriptionAsync(headline.Title)}</div>");
        sb.AppendLine($"<a href=\"{headline.Url}\" target=\"_blank\" class=\"story-link\">{displayUrl}</a>");
        sb.AppendLine("</div>");
        validHeadlineCount++;
      }
    }

    if (validHeadlineCount == 0)
    {
      foreach (var source in websiteData.Take(3))
      {
        sb.AppendLine("<div class=\"story\">");
        sb.AppendLine($"<h4>Latest news from {source.Name}</h4>");
        sb.AppendLine($"<a href=\"{source.Url}\" target=\"_blank\" class=\"story-link\">Visit {source.Name}</a>");
        sb.AppendLine("</div>");
      }
    }
    var subject = $"{topic} News Brief - {countryDisplay} - {DateTime.UtcNow:MMM dd}";
    return new AiSummaryResult(true, subject, sb.ToString());
  }

  private static bool IsValidHeadline(string headline)
  {
    if (string.IsNullOrWhiteSpace(headline)) return false;
    if (headline.Length < 20) return false;
    var wordCount = headline.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    if (wordCount < 4) return false;
    var garbage = new[]
    {
      "more top stories", "top stories", "most watched", "watch today", "have a news tip",
      "have a tip", "breaking news", "latest news", "more stories", "read more",
      "sign up", "sign in", "log in", "subscribe", "newsletter",
      "health & wellness", "sports coverage", "video game reviews", "popular reviews",
      "the athletic", "entertainment", "opinion", "photos", "gallery", "videos",
      "season 2 of", "available now", "watch now", "listen now", "download",
      "exclusively on", "coming soon",
      "guides hub", "release dates", "in review", "year in review",
      "technology review explains", "mit technology review explains",
      "exploring ahead", "bidding farewell"
    };
    var lower = headline.ToLowerInvariant();
    if (garbage.Any(g => lower.Contains(g))) return false;
    if (System.Text.RegularExpressions.Regex.IsMatch(headline, @"^\d{1,2}:\d{2}")) return false;
    if (System.Text.RegularExpressions.Regex.IsMatch(headline, @"^20\d{2}\s")) return false;
    return true;
  }

  private bool IsValidArticleUrl(string url)
  {
    if (string.IsNullOrWhiteSpace(url)) return false;
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
    if (uri.Scheme != "http" && uri.Scheme != "https") return false;
    var host = uri.Host;
    if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)) host = host.Substring(4);
    if (!_allowedDomains.Contains(host)) return false;

    var path = uri.AbsolutePath.ToLowerInvariant();
    var rejectPatterns = new[]
    {
      "/tag/", "/tags/", "/category/", "/categories/", "/author/",
      "/search", "/topic/", "/topics/", "/section/", "/archive",
      "/feed", "/rss", "/login", "/register", "/subscribe",
      "/newsletter", "/about", "/contact", "/privacy", "/terms"
    };
    if (rejectPatterns.Any(p => path.Contains(p))) return false;
    if (string.IsNullOrEmpty(path) || path == "/") return false;
    return true;
  }

  public async Task<string> GenerateDescriptionAsync(string title)
  {
    var prompt = $"Write a short, non-repetitive, engaging description for the title at hand '{title}'. Do not repeat the title. Focus on what kind of news or content a reader can expect from this site. The text you generate needs to contain a maximum of 30 characters";
    var apiKey = _configuration["OpenAI:ApiKey"];
    if (string.IsNullOrEmpty(apiKey)) return $"A source for the latest updates.";
    try
    {
      var client = new ChatClient("gpt-4o-mini", apiKey);
      var response = await client.CompleteChatAsync(prompt);
      return response.Value.Content[0].Text.Trim();
    }
    catch
    {
      return $"A source for the latest updates.";
    }
  }

  public async Task<string> GenerateDescriptionForTopic(string topic)
  {
    var prompt = $"Write a short, engaging, and informative description about the topic '{topic}'. Do not just restate the topic. Explain what a reader can expect to learn or understand about this topic in the news.";
    var apiKey = _configuration["OpenAI:ApiKey"];
    if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY")
      return $"Latest news and insights about {topic}.";
    try
    {
      var client = new ChatClient("gpt-4o-mini", apiKey);
      var response = await client.CompleteChatAsync(prompt);
      return response.Value.Content[0].Text.Trim();
    }
    catch
    {
      return $"Latest news and insights about {topic}.";
    }
  }

  public static string SetStrLimit(string str, int limit)
  {
    if (string.IsNullOrEmpty(str) || limit < 0) return str;
    if (str.Length <= limit) return str;
    return str.Substring(0, limit) + "...";
  }
}

