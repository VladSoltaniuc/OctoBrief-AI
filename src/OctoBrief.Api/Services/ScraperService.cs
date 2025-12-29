using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace OctoBrief.Api.Services;

public class ScraperService : IScraperService
{
  private readonly HttpClient _httpClient;
  private readonly HashSet<string> _allowedDomains;

  // Words that indicate a URL segment is a category/section, not an article
  private static readonly HashSet<string> CategoryWords = new(StringComparer.OrdinalIgnoreCase)
  {
    // Generic categories
    "news", "article", "articles", "story", "stories", "post", "posts",
    "category", "categories", "tag", "tags", "topic", "topics",
    "section", "sections", "archive", "archives", "feed", "rss",
    
    // Site sections
    "home", "about", "contact", "privacy", "terms", "login", "register",
    "subscribe", "newsletter", "search", "author", "authors", "profile",
    
    // Media
    "video", "videos", "gallery", "galleries", "photos", "images", "media",
    
    // Health-specific categories (from health.com example)
    "condition", "conditions", "diseases", "symptoms", "treatments",
    "health", "wellness", "fitness", "nutrition", "diet", "diets",
    "mind", "body", "beauty", "weight", "pregnancy", "parenting",
    
    // Tech categories
    "tech", "technology", "gadgets", "apps", "software", "hardware",
    "gaming", "games", "reviews", "guides", "tutorials", "how-to",
    
    // News categories
    "politics", "business", "sports", "entertainment", "lifestyle",
    "science", "world", "local", "national", "opinion", "editorial",
    
    // Time-based
    "latest", "trending", "popular", "featured", "top", "best",
    "today", "week", "month", "year"
  };

  // Patterns that ALWAYS indicate a real article
  private static readonly Regex[] ArticlePatterns = new[]
  {
    new Regex(@"/\d{4}/\d{2}/\d{2}/", RegexOptions.Compiled),     // /2024/12/29/
    new Regex(@"/\d{4}/\d{2}/", RegexOptions.Compiled),           // /2024/12/
    new Regex(@"/\d{4}-\d{2}-\d{2}", RegexOptions.Compiled),      // -2024-12-29
    new Regex(@"-\d{6,}($|/|\?)", RegexOptions.Compiled),         // article-123456 (ID at end)
    new Regex(@"/\d{6,}($|/|\?)", RegexOptions.Compiled),         // /123456 (numeric ID)
    new Regex(@"\.html?($|\?)", RegexOptions.Compiled),           // .html or .htm
  };

  // Patterns that ALWAYS reject
  private static readonly Regex[] RejectPatterns = new[]
  {
    new Regex(@"^/?$", RegexOptions.Compiled),                    // Homepage
    new Regex(@"linkedin\.com", RegexOptions.IgnoreCase | RegexOptions.Compiled),  // LinkedIn profiles
    new Regex(@"twitter\.com", RegexOptions.IgnoreCase | RegexOptions.Compiled),   // Twitter/X profiles
    new Regex(@"facebook\.com", RegexOptions.IgnoreCase | RegexOptions.Compiled),  // Facebook
    new Regex(@"instagram\.com", RegexOptions.IgnoreCase | RegexOptions.Compiled), // Instagram
    new Regex(@"[#?]", RegexOptions.Compiled),                    // Has query/fragment (usually not articles)
    new Regex(@"\.(jpg|jpeg|png|gif|pdf|mp4|mp3)($|\?)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
    new Regex(@"^javascript:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
    new Regex(@"^mailto:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
  };

  public ScraperService(IHttpClientFactory httpClientFactory)
  {
    _httpClient = httpClientFactory.CreateClient("Scraper");
    _allowedDomains = AllowedDomainsProvider.GetAllowedDomains();
  }

  public async Task<ScrapeResult> ScrapeWebsiteAsync(string url)
  {
    try
    {
      var response = await _httpClient.GetAsync(url);
      response.EnsureSuccessStatusCode();

      var html = await response.Content.ReadAsStringAsync();
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      var baseUri = new Uri(url);
      var homepageUrl = $"{baseUri.Scheme}://{baseUri.Host}";

      var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim()
                  ?? doc.DocumentNode.SelectSingleNode("//h1")?.InnerText?.Trim()
                  ?? "Untitled";

      var headlines = ExtractArticleHeadlines(doc, baseUri, homepageUrl);
      var content = ExtractContent(doc);
      return new ScrapeResult(true, title, headlines, content);
    }
    catch (Exception ex)
    {
      return new ScrapeResult(false, null, [], string.Empty, "500");
    }
  }

  private List<HeadlineWithUrl> ExtractArticleHeadlines(HtmlDocument doc, Uri baseUri, string homepageUrl)
  {
    var headlines = new List<HeadlineWithUrl>();
    var seenTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // Selectors prioritizing links with headlines
    var linkSelectors = new[]
    {
      "//article//a[@href]",
      "//a[.//h1 or .//h2 or .//h3 or .//h4]",
      "//h1/a[@href]", "//h2/a[@href]", "//h3/a[@href]", "//h4/a[@href]",
      "//a[contains(@class, 'article')][@href]",
      "//a[contains(@class, 'headline')][@href]",
      "//a[contains(@class, 'story')][@href]",
      "//a[contains(@class, 'title')][@href]",
      "//a[contains(@class, 'post')][@href]",
      "//*[contains(@class, 'article')]//a[@href]",
      "//*[contains(@class, 'story')]//a[@href]",
      "//*[contains(@class, 'headline')]//a[@href]",
      "//*[contains(@class, 'card')]//a[@href]",
      "//main//a[@href]",
    };

    foreach (var selector in linkSelectors)
    {
      var nodes = doc.DocumentNode.SelectNodes(selector);
      if (nodes == null) continue;

      foreach (var node in nodes)
      {
        var href = node.GetAttributeValue("href", "");
        if (string.IsNullOrWhiteSpace(href)) continue;

        var articleUrl = ResolveUrl(href, baseUri);
        if (string.IsNullOrEmpty(articleUrl)) continue;
        if (!seenUrls.Add(articleUrl)) continue;

        // Score this URL - higher score = more likely to be an article
        var score = ScoreArticleUrl(articleUrl, baseUri);
        if (score < 1) continue; // Minimum score threshold - lower to allow simpler URL structures

        var headlineText = ExtractHeadlineText(node);
        if (string.IsNullOrWhiteSpace(headlineText)) continue;
        if (!IsValidHeadline(headlineText)) continue;
        if (!seenTitles.Add(headlineText)) continue;

        headlines.Add(new HeadlineWithUrl(headlineText, articleUrl));
        if (headlines.Count >= 15) break;
      }
      if (headlines.Count >= 15) break;
    }

    // Broader search if we found too few
    if (headlines.Count < 3)
    {
      var allLinks = doc.DocumentNode.SelectNodes("//a[@href]");
      if (allLinks != null)
      {
        foreach (var node in allLinks)
        {
          if (headlines.Count >= 15) break;

          var href = node.GetAttributeValue("href", "");
          if (string.IsNullOrWhiteSpace(href)) continue;

          var articleUrl = ResolveUrl(href, baseUri);
          if (string.IsNullOrEmpty(articleUrl)) continue;
          if (!seenUrls.Add(articleUrl)) continue;

          var score = ScoreArticleUrl(articleUrl, baseUri);
          if (score < 1) continue; // Lower threshold for broader search

          var headlineText = ExtractHeadlineText(node);
          if (string.IsNullOrWhiteSpace(headlineText)) continue;
          if (!IsValidHeadline(headlineText)) continue;
          if (!seenTitles.Add(headlineText)) continue;

          headlines.Add(new HeadlineWithUrl(headlineText, articleUrl));
        }
      }
    }
    // Fallback to homepage
    if (headlines.Count == 0) headlines.Add(new HeadlineWithUrl($"Latest news from {baseUri.Host}", homepageUrl));
    return headlines;
  }

  private int ScoreArticleUrl(string url, Uri baseUri)
  {
    try
    {
      var uri = new Uri(url);
      var path = uri.AbsolutePath;

      foreach (var pattern in RejectPatterns)
      {
        if (pattern.IsMatch(path) || pattern.IsMatch(url)) return 0;
      }

      int score = 0;
      foreach (var pattern in ArticlePatterns)
      {
        if (pattern.IsMatch(path))
        {
          score += 5;
          break;
        }
      }

      var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
      if (segments.Length == 0) return 0; // Homepage

      var lastSegment = segments[^1];
      var lastSegmentWords = lastSegment.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

      // Depth bonus: more segments usually means more specific content
      // /condition/allergy = 2 segments (likely category)
      // /condition/allergy/allergy-shots = 3 segments (likely article)
      if (segments.Length >= 3) score += 2;
      else if (segments.Length == 2) score += 1;
      else if (segments.Length == 1)
      {
        var segment = segments[0];
        if (segment.EndsWith(".php", StringComparison.OrdinalIgnoreCase) ||
            segment.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            segment.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
          score += 2;
        else if (Regex.IsMatch(segment, @"[-_]?[0-9A-Z]{5,}$", RegexOptions.IgnoreCase)) score += 3;
        else if (lastSegmentWords.Length >= 4) score += 2;
      }

      if (CategoryWords.Contains(lastSegment)) score -= 2;
      else if (lastSegmentWords.Length >= 3) score += 3;
      else if (lastSegmentWords.Length == 2) score += 1;
      else if (lastSegmentWords.Length == 1 && !CategoryWords.Contains(lastSegmentWords[0]) && Regex.IsMatch(lastSegment, @"\d")) score += 2;

      if (Regex.IsMatch(path, @"20\d{2}")) score += 2;
      if (Regex.IsMatch(lastSegment, @"\d{5,}")) score += 3;

      // Penalize if path looks like a category listing
      // e.g., /diseases-conditions, /health-news, /fitness-tips
      if (segments.Length == 1) if (!Regex.IsMatch(lastSegment, @"\d{5,}")) score -= 1;

      // Check for common article URL patterns
      if (path.Contains("/article/") || path.Contains("/story/") || path.Contains("/news/") || path.Contains("/post/")) score += 2;

      // Bonus for very long slugs (articles tend to have descriptive URLs)
      if (lastSegment.Length > 30) score += 1;
      return score;
    }
    catch
    {
      return 0;
    }
  }

  private string ExtractHeadlineText(HtmlNode linkNode)
  {
    // Try headline elements first
    var headlineNode = linkNode.SelectSingleNode(".//h1|.//h2|.//h3|.//h4|.//h5|.//h6");
    if (headlineNode != null)
    {
      var text = CleanText(headlineNode.InnerText);
      if (!string.IsNullOrWhiteSpace(text) && text.Length >= 10) return text;
    }

    // Try elements with headline-like classes
    var titleNode = linkNode.SelectSingleNode(".//*[contains(@class, 'title') or contains(@class, 'headline')]");
    if (titleNode != null)
    {
      var text = CleanText(titleNode.InnerText);
      if (!string.IsNullOrWhiteSpace(text) && text.Length >= 10) return text;
    }
    return CleanText(linkNode.InnerText);
  }

  private bool IsValidHeadline(string text)
  {
    if (string.IsNullOrWhiteSpace(text)) return false;
    if (text.Length < 15 || text.Length > 300) return false;

    var lowerText = text.ToLowerInvariant();

    // Skip navigation/UI elements
    var skipPhrases = new[]
    {
      "read more", "click here", "learn more", "view all", "see more", "see all",
      "subscribe", "sign up", "log in", "register", "newsletter",
      "privacy policy", "terms of service", "cookie policy", "contact us",
      "about us", "advertise", "careers", "copyright", "all rights reserved",
      "follow us", "share this", "comments", "reply", "next page",
      "previous", "load more", "show more", "expand", "menu",
      "search", "back to", "skip to", "jump to",
      "top stories", "trending", "popular", "most read", "editor's picks",
      "sponsored", "advertisement", "promoted", "partner content",
    };

    foreach (var phrase in skipPhrases)
    {
      if (lowerText.Contains(phrase)) return false;
    }

    // Skip if mostly numbers/symbols
    var letterCount = text.Count(char.IsLetter);
    if (letterCount < text.Length * 0.5) return false;

    // Should have at least 3 words
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (words.Length < 3) return false;

    return true;
  }

  private bool IsAllowedDomain(string url)
  {
    try
    {
      var uri = new Uri(url);
      var host = uri.Host;
      // Strip www. prefix for comparison
      if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        host = host.Substring(4);
      return _allowedDomains.Contains(host);
    }
    catch
    {
      return false;
    }
  }

  private string ResolveUrl(string href, Uri baseUri)
  {
    if (href.StartsWith("//"))
    {
      var url = $"{baseUri.Scheme}:{href}";
      return IsAllowedDomain(url) ? url : string.Empty;
    }

    if (Uri.TryCreate(href, UriKind.Absolute, out var absoluteUri))
    {
      if (absoluteUri.Scheme == "http" || absoluteUri.Scheme == "https")
      {
        var url = absoluteUri.ToString();
        return IsAllowedDomain(url) ? url : string.Empty;
      }
      return string.Empty;
    }

    if (Uri.TryCreate(baseUri, href, out var resolvedUri))
    {
      var url = resolvedUri.ToString();
      return IsAllowedDomain(url) ? url : string.Empty;
    }
    return string.Empty;
  }

  private string ExtractContent(HtmlDocument doc)
  {
    var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//nav|//header|//footer|//aside|//noscript|//iframe|//form");
    if (nodesToRemove != null) foreach (var node in nodesToRemove.ToList()) node.Remove();
    var sb = new StringBuilder();
    var mainContent = doc.DocumentNode.SelectSingleNode("//main")
                     ?? doc.DocumentNode.SelectSingleNode("//article")
                     ?? doc.DocumentNode.SelectSingleNode("//*[@role='main']")
                     ?? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'content')]")
                     ?? doc.DocumentNode.SelectSingleNode("//body");

    if (mainContent == null) return sb.ToString().Trim();
    var paragraphs = mainContent.SelectNodes(".//p");
    if (paragraphs == null) return sb.ToString().Trim();
    foreach (var p in paragraphs)
    {
      var text = CleanText(p.InnerText);
      if (!string.IsNullOrWhiteSpace(text) && text.Length > 30)
      {
        sb.AppendLine(text);
        sb.AppendLine();
      }
    }
    return sb.ToString().Trim();
  }

  private string CleanText(string text)
  {
    if (string.IsNullOrEmpty(text)) return string.Empty;
    text = System.Net.WebUtility.HtmlDecode(text);
    text = Regex.Replace(text, @"\s+", " ");
    return text.Trim();
  }
}
