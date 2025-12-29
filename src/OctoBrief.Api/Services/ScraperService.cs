using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace OctoBrief.Api.Services;

public class ScraperService : IScraperService
{
  private readonly ILogger<ScraperService> _logger;
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

  public ScraperService(ILogger<ScraperService> logger, IHttpClientFactory httpClientFactory)
  {
    _logger = logger;
    _httpClient = httpClientFactory.CreateClient("Scraper");

    // Allowed domains from NewsSearchService sources
    _allowedDomains = GetAllowedDomains();
    _logger.LogInformation("Initialized scraper with {Count} allowed domains", _allowedDomains.Count);
  }

  private static HashSet<string> GetAllowedDomains()
  {
    // Extract domains from all news sources defined in NewsSearchService
    return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
      // USA
      "techcrunch.com", "theverge.com", "arstechnica.com", "wired.com", "engadget.com",
      "scientificamerican.com", "sciencedaily.com", "livescience.com", "space.com",
      "espn.com", "bleacherreport.com", "si.com", "theathletic.com", "cbssports.com",
      "variety.com", "hollywoodreporter.com", "deadline.com", "ew.com", "rollingstone.com",
      "webmd.com", "healthline.com", "statnews.com", "health.com",
      "insideclimatenews.org", "grist.org", "e360.yale.edu", "climatecentral.org",
      "politico.com", "thehill.com", "axios.com", "cnn.com", "foxnews.com",
      "coindesk.com", "theblock.co", "decrypt.co", "bitcoinmagazine.com",
      "ign.com", "gamespot.com", "kotaku.com", "polygon.com", "pcgamer.com",
      // UK
      "theregister.com", "techradar.com", "wired.co.uk", "bbc.com", "bbc.co.uk",
      "newscientist.com", "nature.com", "theguardian.com",
      "skysports.com", "talksport.com",
      "digitalspy.com", "nme.com", "radiotimes.com",
      "nhs.uk",
      "carbonbrief.org", "climatechangenews.com",
      "news.sky.com", "politico.eu",
      "cityam.com",
      "eurogamer.net", "rockpapershotgun.com", "gamesradar.com", "nintendolife.com",
      // Canada
      "betakit.com", "itworldcanada.com", "mobilesyrup.com", "dailyhive.com",
      "cbc.ca", "canadiangeographic.ca", "science.gc.ca",
      "tsn.ca", "sportsnet.ca",
      "etcanada.com", "thestar.com",
      "healthing.ca", "globalnews.ca",
      "nationalobserver.com", "thenarwhal.ca",
      "theglobeandmail.com", "ipolitics.ca",
      "cgmagonline.com",
      // Germany
      "heise.de", "golem.de", "t3n.de", "chip.de",
      "spektrum.de", "scinexx.de", "wissenschaft.de",
      "kicker.de", "sport1.de", "sportschau.de", "spox.com",
      "dwdl.de", "filmstarts.de", "rollingstone.de",
      "apotheken-umschau.de", "netdoktor.de", "gesundheit.de",
      "klimareporter.de", "utopia.de", "spiegel.de",
      "tagesschau.de", "zeit.de", "faz.net",
      "btc-echo.de", "de.cointelegraph.com", "blocktrainer.de",
      "gamestar.de", "pcgames.de", "4players.de", "spieletipps.de",
      // France
      "01net.com", "numerama.com", "frandroid.com", "lesnumeriques.com",
      "sciencesetavenir.fr", "futura-sciences.com", "pourlascience.fr",
      "lequipe.fr", "rmcsport.bfmtv.com", "eurosport.fr", "sofoot.com",
      "allocine.fr", "premiere.fr", "telerama.fr",
      "doctissimo.fr", "santemagazine.fr", "topsante.com",
      "novethic.fr", "vert.eco", "actu-environnement.com",
      "francetvinfo.fr", "lemonde.fr", "lefigaro.fr",
      "journalducoin.com", "cryptonaute.fr", "cointribune.com",
      "jeuxvideo.com", "gamekult.com", "jeuxactu.com", "nofrag.com",
      // Italy
      "tomshw.it", "hdblog.it", "punto-informatico.it", "wired.it",
      "lescienze.it", "focus.it", "galileonet.it",
      "gazzetta.it", "corrieredellosport.it", "tuttosport.com", "sport.sky.it",
      "comingsoon.it", "badtaste.it", "rockol.it",
      "corriere.it", "humanitasalute.it", "ok-salute.it",
      "rinnovabili.it", "qualenergia.it", "greenreport.it",
      "ansa.it", "repubblica.it", "ilpost.it",
      "cryptonomist.ch", "criptovaluta.it", "it.cointelegraph.com",
      "multiplayer.it", "everyeye.it", "spaziogames.it", "it.ign.com",
      // Spain
      "xataka.com", "genbeta.com", "elandroidelibre.elespanol.com", "hipertextual.com",
      "muyinteresante.es", "nationalgeographic.com.es", "agenciasinc.es",
      "marca.com", "as.com", "mundodeportivo.com", "sport.es",
      "sensacine.com", "ecartelera.com", "formulatv.com",
      "cuidateplus.marca.com", "webconsultas.com", "infosalus.com",
      "climatica.lamarea.com", "ecologistasenaccion.org", "elperiodicodelaenergia.com",
      "elpais.com", "elmundo.es", "lavanguardia.com",
      "es.cointelegraph.com", "news.bit2me.com", "observatorioblockchain.com",
      "3djuegos.com", "vandal.elespanol.com", "meristation.as.com", "vidaextra.com",
      // Romania
      "playtech.ro", "go4it.ro", "start-up.ro", "techcafe.ro",
      "descopera.ro", "sciencealert.ro", "natgeo.ro",
      "gsp.ro", "prosport.ro", "sport.ro", "digisport.ro",
      "cinemagia.ro", "adevarul.ro", "observator.tv",
      "romedic.ro", "sfatulmedicului.ro", "viata-medicala.ro",
      "greenpeace.org", "digi24.ro",
      "hotnews.ro", "g4media.ro",
      "crypto.ro",
      "go4games.ro", "need4games.ro",
      // Poland
      "antyweb.pl", "chip.pl", "komputerswiat.pl", "benchmark.pl",
      "naukawpolsce.pl", "crazynauka.pl", "national-geographic.pl",
      "sport.pl", "sportowefakty.wp.pl", "przegladsportowy.pl", "sport.interia.pl",
      "filmweb.pl", "kultura.onet.pl", "film.wp.pl",
      "medonet.pl", "poradnikzdrowie.pl", "zdrowie.pap.pl",
      "zielona.interia.pl", "hightech.fm", "wyborcza.pl",
      "tvn24.pl", "wiadomosci.onet.pl", "rp.pl",
      "bitcoin.pl", "kryptowaluty.pl", "cryptonews.pl",
      "gry-online.pl", "cdaction.pl", "ppe.pl", "gram.pl",
      // Common TLDs
      "cointelegraph.com"
    };
  }

  public async Task<ScrapeResult> ScrapeWebsiteAsync(string url)
  {
    try
    {
      _logger.LogInformation("Scraping website: {Url}", url);

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

      _logger.LogInformation("Scraped {Url}: Found {HeadlineCount} article headlines, {ContentLength} chars",
        url, headlines.Count, content.Length);

      return new ScrapeResult(true, title, headlines, content);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to scrape {Url}", url);
      return new ScrapeResult(false, null, [], string.Empty, ex.Message);
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
        _logger.LogDebug("URL: {Url} scored {Score}", articleUrl, score);
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
      _logger.LogInformation("Few headlines found ({Count}), trying broader search for {Host}", headlines.Count, baseUri.Host);

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
    if (headlines.Count == 0)
    {
      _logger.LogWarning("No article headlines found for {Host}, using homepage fallback", baseUri.Host);
      headlines.Add(new HeadlineWithUrl($"Latest news from {baseUri.Host}", homepageUrl));
    }

    return headlines;
  }

  /// <summary>
  /// Scores a URL to determine how likely it is to be an actual article page.
  /// Higher score = more likely to be a real article with content.
  /// 
  /// Examples from health.com:
  /// - https://www.health.com/ (homepage) → score 0
  /// - https://www.health.com/condition/allergy (category) → score 1-2
  /// - https://www.health.com/diseases-conditions (category listing) → score 0-1
  /// - https://www.health.com/eat-well-6951509 (article with ID) → score 5+
  /// - https://www.health.com/diets/best-diets-for-2022 (article with year) → score 4+
  /// - https://www.health.com/condition/allergy/allergy-shots (deep article) → score 4+
  /// </summary>
  private int ScoreArticleUrl(string url, Uri baseUri)
  {
    try
    {
      var uri = new Uri(url);
      var path = uri.AbsolutePath;

      // Immediate rejection patterns
      foreach (var pattern in RejectPatterns)
      {
        if (pattern.IsMatch(path) || pattern.IsMatch(url))
          return 0;
      }

      int score = 0;

      // Strong article indicators (immediate high score)
      foreach (var pattern in ArticlePatterns)
      {
        if (pattern.IsMatch(path))
        {
          score += 5;
          break;
        }
      }

      // Analyze path segments
      var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

      if (segments.Length == 0)
        return 0; // Homepage

      // Analyze last segment for word count
      var lastSegment = segments[^1];
      var lastSegmentWords = lastSegment.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

      // Depth bonus: more segments usually means more specific content
      // /condition/allergy = 2 segments (likely category)
      // /condition/allergy/allergy-shots = 3 segments (likely article)
      if (segments.Length >= 3)
        score += 2;
      else if (segments.Length == 2)
        score += 1;
      else if (segments.Length == 1)
      {
        // Single segment - give bonus for file extensions or IDs that indicate content
        var segment = segments[0];
        if (segment.EndsWith(".php", StringComparison.OrdinalIgnoreCase) ||
            segment.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            segment.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
          score += 2; // PHP/HTML files could be articles
        // Check for IDs at the end (like 0P54516, -123456, etc.)
        else if (System.Text.RegularExpressions.Regex.IsMatch(segment, @"[-_]?[0-9A-Z]{5,}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
          score += 3; // Has ID at the end
        else if (lastSegmentWords.Length >= 4)
          score += 2; // Long hyphenated slug likely an article
      }

      // Check if last segment is a category word (bad sign)
      if (CategoryWords.Contains(lastSegment))
      {
        score -= 2;
      }
      // Check if last segment is hyphenated with multiple words (good sign - article slug)
      else if (lastSegmentWords.Length >= 3)
      {
        score += 3; // e.g., "best-diets-for-2022" or "allergy-shots-treatment-guide"
      }
      else if (lastSegmentWords.Length == 2)
      {
        score += 1; // e.g., "allergy-shots"
      }
      else if (lastSegmentWords.Length == 1 && !CategoryWords.Contains(lastSegmentWords[0]))
      {
        // Single word that's not a category - could be an article or category
        // Check if it looks like a slug (has numbers or is very specific)
        if (Regex.IsMatch(lastSegment, @"\d"))
          score += 2; // Has numbers, likely article ID
      }

      // Check for year in URL (articles often have years)
      if (Regex.IsMatch(path, @"20\d{2}"))
        score += 2;

      // Check for numeric ID anywhere in URL
      if (Regex.IsMatch(lastSegment, @"\d{5,}"))
        score += 3;

      // Penalize if path looks like a category listing
      // e.g., /diseases-conditions, /health-news, /fitness-tips
      if (segments.Length == 1)
      {
        // Single segment paths are almost always categories unless they have an ID
        if (!Regex.IsMatch(lastSegment, @"\d{5,}"))
          score -= 1;
      }

      // Check for common article URL patterns
      if (path.Contains("/article/") || path.Contains("/story/") ||
          path.Contains("/news/") || path.Contains("/post/"))
        score += 2;

      // Bonus for very long slugs (articles tend to have descriptive URLs)
      if (lastSegment.Length > 30)
        score += 1;

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
      if (!string.IsNullOrWhiteSpace(text) && text.Length >= 10)
        return text;
    }

    // Try elements with headline-like classes
    var titleNode = linkNode.SelectSingleNode(".//*[contains(@class, 'title') or contains(@class, 'headline')]");
    if (titleNode != null)
    {
      var text = CleanText(titleNode.InnerText);
      if (!string.IsNullOrWhiteSpace(text) && text.Length >= 10)
        return text;
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
    try
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
    }
    catch { }

    return string.Empty;
  }

  private string ExtractContent(HtmlDocument doc)
  {
    var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//nav|//header|//footer|//aside|//noscript|//iframe|//form");
    if (nodesToRemove != null)
    {
      foreach (var node in nodesToRemove.ToList())
        node.Remove();
    }

    var sb = new StringBuilder();
    var mainContent = doc.DocumentNode.SelectSingleNode("//main")
                     ?? doc.DocumentNode.SelectSingleNode("//article")
                     ?? doc.DocumentNode.SelectSingleNode("//*[@role='main']")
                     ?? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'content')]")
                     ?? doc.DocumentNode.SelectSingleNode("//body");

    if (mainContent != null)
    {
      var paragraphs = mainContent.SelectNodes(".//p");
      if (paragraphs != null)
      {
        foreach (var p in paragraphs)
        {
          var text = CleanText(p.InnerText);
          if (!string.IsNullOrWhiteSpace(text) && text.Length > 30)
          {
            sb.AppendLine(text);
            sb.AppendLine();
          }
        }
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
