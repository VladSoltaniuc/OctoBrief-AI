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

  public async Task<AiSummaryResult> SummarizeContentAsync(string websiteName, List<string> headlines, string content, List<string>? sourceUrls = null, string? topic = null)
  {
    var apiKey = _configuration["OpenAI:ApiKey"];

    if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY")
      return GenerateFallbackSummary(websiteName, headlines, content, sourceUrls);

    try
    {
      var client = new ChatClient("gpt-4o-mini", apiKey);
      var headlinesList = string.Join("\n", headlines.Take(20).Select((h, i) => $"{i + 1}. {h}"));
      var truncatedContent = content.Length > 8000 ? content[..8000] + "..." : content;
      var sourceUrlsInfo = sourceUrls != null && sourceUrls.Count > 0
        ? $"Source URLs: {string.Join(", ", sourceUrls)}"
        : "";

      var topicFilter = !string.IsNullOrWhiteSpace(topic)
        ? $"\n\nIMPORTANT: The user is specifically interested in the topic: \"{topic}\". Filter and prioritize headlines related to this topic. If no headlines match the topic exactly, find the closest related ones."
        : "";

      var prompt = $"""
        You are a professional news summarizer and editor. Create a brief email digest based on the following content from "{websiteName}".
        {topicFilter}

        HEADLINES FROM THE WEBSITE:
        {headlinesList}

        CONTENT:
        {truncatedContent}

        {sourceUrlsInfo}

        INSTRUCTIONS:
        1. Select the TOP 3 most important/newsworthy headlines{(!string.IsNullOrWhiteSpace(topic) ? $" related to \"{topic}\"" : "")}
        2. For each headline, create a SHORT, INFORMATIVE title (max 10-15 words) - DO NOT copy-paste the original headline verbatim, rewrite it to be clear and concise
        3. Write a 2-3 sentence summary for each headline that explains the key points in simple terms
        4. Include a "Read More" link for each headline pointing to the source website

        Format your response as HTML suitable for an email body. Use this exact structure:
        - <h2> for "Today's Top Stories" section title
        - For each of the 3 stories use:
          <div class="story">
            <h3>[Your rewritten short title]</h3>
            <p>[2-3 sentence summary explaining the news in simple terms]</p>
            <a href="[source_url]" class="read-more">Read More →</a>
          </div>

        The source URL for "Read More" should be: {(sourceUrls?.FirstOrDefault() ?? "#")}

        Keep it concise, professional, and easy to understand. News websites often write confusing headlines - your job is to make them clear.
        Do NOT include any HTML, HEAD, or BODY tags - just the content.
        """;

      var response = await client.CompleteChatAsync(prompt);
      var htmlContent = response.Value.Content[0].Text;
      var subjectTopicPart = !string.IsNullOrWhiteSpace(topic) ? $" on {topic}" : "";
      var subjectPrompt = $"Generate a short, catchy email subject line (max 60 chars) for a news digest{subjectTopicPart} from {websiteName}. Just the subject, no quotes.";
      var subjectResponse = await client.CompleteChatAsync(subjectPrompt);
      var subject = subjectResponse.Value.Content[0].Text.Trim().Trim('"');
      return new AiSummaryResult(true, subject, WrapInEmailTemplate(websiteName, htmlContent));
    }
    catch (Exception ex)
    {
      return GenerateFallbackSummary(websiteName, headlines, content, sourceUrls);
    }
  }

  public async Task<AiSummaryResult> SummarizeMultiSourceNewsAsync(string topic, string country, List<WebsiteNewsData> websiteData)
  {
    var apiKey = _configuration["OpenAI:ApiKey"];

    if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY")
      return GenerateMultiSourceFallbackSummary(topic, country, websiteData);

    try
    {
      var client = new ChatClient("gpt-4o-mini", apiKey);
      var sourcesInfo = new StringBuilder();
      foreach (var source in websiteData)
      {
        var outletType = source.IsMajorOutlet ? "Major News Outlet" : "Specialized/Independent Source";
        sourcesInfo.AppendLine($"\n=== {source.Name} ({outletType}) ===");
        sourcesInfo.AppendLine($"Site URL: {source.Url}");
        sourcesInfo.AppendLine("Headlines with article URLs:");
        var filteredHeadlines = source.Headlines
          .Where(h => IsValidHeadline(h.Title) && IsValidArticleUrl(h.Url))
          .Take(10);

        foreach (var headline in filteredHeadlines)
        {
          sourcesInfo.AppendLine($"  - \"{headline.Title}\" => {headline.Url}");
        }
        if (!string.IsNullOrEmpty(source.Content))
        {
          var preview = source.Content.Length > 2000 ? source.Content[..2000] + "..." : source.Content;
          sourcesInfo.AppendLine($"Content Preview: {preview}");
        }
      }
      var countryDisplay = country.Equals("global", StringComparison.OrdinalIgnoreCase) ? "Global" : country;
      var majorCount = websiteData.Count(w => w.IsMajorOutlet);
      var smallerCount = websiteData.Count(w => !w.IsMajorOutlet);
      var prompt = $"""
        You are a strict news curator. Create a brief about "{topic}" for {countryDisplay}.

        CRITICAL - ONLY include headlines that are ACTUAL NEWS ARTICLES about "{topic}":
        - The headline must describe a specific news event, development, or story
        - The subject must be predominantly about "{topic}"
        - Skip anything that looks like navigation, categories, or website UI elements
        - ONLY use headlines that have a real article URL (not just the site homepage)

        ABSOLUTELY REJECT these (output NOTHING for them):
        - Navigation/UI: "More stories", "Top stories", "Watch videos", "Have a tip", "Health & Wellness", "Sports coverage"
        - Promotional content: "Subscribe", "Sign up", "Season 2 of...", "Available now"
        - Generic labels: "Popular Reviews", "Video Game Reviews", "Breaking News"
        - Date/time references without news: "2025 IN REVIEW", "December", timestamps like "02:47"
        - Anything under 6 words that isn't a clear news headline

        NEWS DATA (format: "Headline" => article_url):
        {sourcesInfo}

        OUTPUT FORMAT - For EACH valid news headline, output EXACTLY:
        <div class="story">
          <h4>[EXACT headline - unchanged]</h4>
          <a href="[THE ARTICLE URL from the data]" target="_blank" class="story-link">[short domain like pcgamer.com]</a>
        </div>

        STRICT RULES:
        - Use the SPECIFIC ARTICLE URL provided for each headline, NOT the site homepage
        - Maximum 2 headlines per source that are REAL NEWS about "{topic}"
        - If a source has NO relevant news, output NOTHING for it
        - Better to output fewer high-quality results than garbage
        - Output ONLY story divs, no wrappers
        """;

      var response = await client.CompleteChatAsync(prompt);
      var htmlContent = response.Value.Content[0].Text;

      // Generate subject line
      var subjectPrompt = $"Generate a short, catchy email subject line (max 60 chars) for a {countryDisplay} news digest about {topic}. Just the subject, no quotes.";
      var subjectResponse = await client.CompleteChatAsync(subjectPrompt);
      var subject = subjectResponse.Value.Content[0].Text.Trim().Trim('"');
      return new AiSummaryResult(true, subject, WrapInMultiSourceEmailTemplate(topic, countryDisplay, htmlContent));
    }
    catch (Exception ex)
    {
      return GenerateMultiSourceFallbackSummary(topic, country, websiteData);
    }
  }

  private AiSummaryResult GenerateMultiSourceFallbackSummary(string topic, string country, List<WebsiteNewsData> websiteData)
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
        sb.AppendLine($"<h4>{headline.Title}</h4>");
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

  private AiSummaryResult GenerateFallbackSummary(string websiteName, List<string> headlines, string content, List<string>? sourceUrls)
  {
    var sb = new StringBuilder();
    var sourceUrl = sourceUrls?.FirstOrDefault() ?? "#";

    sb.AppendLine($"<h2>📰 News Digest from {websiteName}</h2>");
    sb.AppendLine("<p>Here are the latest headlines we found:</p>");
    sb.AppendLine("<h3>Top Headlines</h3>");

    foreach (var headline in headlines.Take(3))
    {
      sb.AppendLine("<div class=\"story\">");
      sb.AppendLine($"<h4><strong>{headline}</strong></h4>");
      sb.AppendLine($"<a href=\"{sourceUrl}\" class=\"read-more\">Read More →</a>");
      sb.AppendLine("</div>");
    }

    if (!string.IsNullOrEmpty(content))
    {
      var preview = content.Length > 300 ? content[..300] + "..." : content;
      sb.AppendLine("<h3>Content Preview</h3>");
      sb.AppendLine($"<p>{preview}</p>");
    }
    var subject = $"Your {websiteName} News Brief - {DateTime.UtcNow:MMM dd}";
    return new AiSummaryResult(true, subject, WrapInEmailTemplate(websiteName, sb.ToString()));
  }

  private string WrapInEmailTemplate(string websiteName, string content)
  {
    return @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <style>
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
      line-height: 1.6;
      color: #333;
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
      background-color: #f5f5f5;
    }
    .container {
      background-color: #ffffff;
      border-radius: 8px;
      padding: 30px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    h2 {
      color: #1a1a1a;
      border-bottom: 2px solid #007bff;
      padding-bottom: 10px;
    }
    h3 {
      color: #333;
      margin-bottom: 8px;
      font-size: 18px;
    }
    h4 {
      color: #444;
      margin-bottom: 5px;
    }
    .story {
      background-color: #f8f9fa;
      border-left: 4px solid #007bff;
      padding: 15px 20px;
      margin-bottom: 20px;
      border-radius: 0 8px 8px 0;
    }
    .story h3 {
      margin-top: 0;
      color: #1a1a1a;
    }
    .story p {
      margin: 10px 0;
      color: #555;
    }
    .read-more {
      display: inline-block;
      color: #007bff;
      text-decoration: none;
      font-weight: 600;
      font-size: 14px;
      margin-top: 8px;
    }
    .read-more:hover {
      text-decoration: underline;
    }
    ul {
      padding-left: 20px;
    }
    li {
      margin-bottom: 12px;
    }
    .footer {
      margin-top: 30px;
      padding-top: 20px;
      border-top: 1px solid #eee;
      font-size: 12px;
      color: #888;
      text-align: center;
    }
    .logo {
      font-size: 24px;
      font-weight: bold;
      color: #007bff;
      margin-bottom: 20px;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""logo"">🐙 OctoBrief</div>
    " + content + @"
    <div class=""footer"">
      <p>This digest was generated by OctoBrief</p>
      <p>Source: " + websiteName + @"</p>
    </div>
  </div>
</body>
</html>";
  }

  private string WrapInMultiSourceEmailTemplate(string topic, string country, string content)
  {
    return @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <style>
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
      line-height: 1.6;
      color: #333;
      max-width: 650px;
      margin: 0 auto;
      padding: 20px;
      background-color: #f5f5f5;
    }
    .container {
      background-color: #ffffff;
      border-radius: 12px;
      padding: 30px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }
    .logo {
      font-size: 24px;
      font-weight: bold;
      color: #007bff;
      margin-bottom: 10px;
    }
    h2 {
      color: #1a1a1a;
      border-bottom: 3px solid #007bff;
      padding-bottom: 12px;
      margin-top: 0;
    }
    .intro {
      color: #666;
      font-size: 16px;
      margin-bottom: 25px;
    }
    .source-section {
      margin-bottom: 30px;
      padding-bottom: 20px;
      border-bottom: 1px solid #eee;
    }
    .source-section:last-child {
      border-bottom: none;
    }
    .source-name {
      color: #333;
      font-size: 20px;
      margin-bottom: 15px;
      display: flex;
      align-items: center;
      gap: 10px;
    }
    .source-type {
      font-size: 12px;
      font-weight: normal;
      color: #fff;
      background-color: #6c757d;
      padding: 3px 10px;
      border-radius: 12px;
    }
    .source-section:nth-child(odd) .source-type {
      background-color: #28a745;
    }
    h3 {
      color: #333;
      margin-bottom: 8px;
      font-size: 18px;
    }
    h4 {
      color: #1a1a1a;
      margin: 0 0 8px 0;
      font-size: 16px;
    }
    .story {
      background-color: #f8f9fa;
      border-left: 4px solid #007bff;
      padding: 15px 20px;
      margin-bottom: 15px;
      border-radius: 0 8px 8px 0;
    }
    .story p {
      margin: 10px 0;
      color: #555;
      font-size: 14px;
    }
    .read-more {
      display: inline-block;
      color: #007bff;
      text-decoration: none;
      font-weight: 600;
      font-size: 13px;
      margin-top: 5px;
    }
    .read-more:hover {
      text-decoration: underline;
    }
    .footer {
      margin-top: 30px;
      padding-top: 20px;
      border-top: 2px solid #eee;
      font-size: 12px;
      color: #888;
      text-align: center;
    }
    .topic-badge {
      display: inline-block;
      background: linear-gradient(135deg, #007bff, #0056b3);
      color: white;
      padding: 5px 15px;
      border-radius: 20px;
      font-size: 14px;
      margin-bottom: 15px;
    }
    .country-badge {
      display: inline-block;
      background-color: #6c757d;
      color: white;
      padding: 3px 12px;
      border-radius: 15px;
      font-size: 12px;
      margin-left: 10px;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""logo"">🐙 OctoBrief</div>
    " + content + @"
    <div class=""footer"">
      <p>This digest was generated by OctoBrief</p>
      <p>Topic: " + topic + @" | Region: " + country + @"</p>
      <p>Generated on " + DateTime.UtcNow.ToString("MMMM dd, yyyy") + @"</p>
    </div>
  </div>
</body>
</html>";
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
}

