using System.Text.RegularExpressions;
using System.Web;

namespace OctoBrief.Api.Security;

public static class HtmlSecurity
{
  public static bool MaliciousContent(string? html)
  {
    if (string.IsNullOrEmpty(html))
      return false;

    // Decode HTML entities to catch encoded attacks
    string decoded = DecodeHtmlEntities(html);

    // Normalize whitespace (newlines, tabs, etc.) to catch obfuscation
    string normalized = NormalizeWhitespace(decoded);

    // Check both original and normalized versions
    return CheckPatterns(html) || CheckPatterns(decoded) || CheckPatterns(normalized);
  }

  private static bool CheckPatterns(string content)
  {
    // Script tags (with variations)
    if (Regex.IsMatch(content, "<\\s*script\\b", RegexOptions.IgnoreCase))
      return true;

    // Nested/broken script tags: <scr<script>ipt>
    if (Regex.IsMatch(content, "<\\s*scr.*?ipt.*?>", RegexOptions.IgnoreCase))
      return true;

    // Event handler attributes: onclick, onerror, onload, onmouseover, etc.
    if (Regex.IsMatch(content, "\\bon[a-z]+\\s*[=\\s]", RegexOptions.IgnoreCase))
      return true;

    // JavaScript protocol in various contexts
    // Catches: javascript:, java script:, jav&#x09;ascript:, etc.
    if (Regex.IsMatch(content, "j\\s*a\\s*v\\s*a\\s*s\\s*c\\s*r\\s*i\\s*p\\s*t\\s*:", RegexOptions.IgnoreCase))
      return true;

    // VBScript (older IE vulnerability)
    if (Regex.IsMatch(content, "vbscript\\s*:", RegexOptions.IgnoreCase))
      return true;

    // Data URIs with executable content
    if (Regex.IsMatch(content, "data:\\s*[^,]*?(text/html|text/javascript|application/(x-)?javascript|image/svg\\+xml)", RegexOptions.IgnoreCase))
      return true;

    // Base64 data URIs (any data URI with base64 encoding could be suspicious)
    if (Regex.IsMatch(content, "data:[^;]*;\\s*base64,[A-Za-z0-9+/=]{100,}", RegexOptions.IgnoreCase))
      return true;

    // Dangerous tags: iframe, object, embed, frame, frameset, applet
    if (Regex.IsMatch(content, "<\\s*(iframe|object|embed|frame|frameset|applet)\\b", RegexOptions.IgnoreCase))
      return true;

    // Meta refresh (can redirect to malicious sites)
    if (Regex.IsMatch(content, "<\\s*meta[^>]*?http-equiv\\s*=\\s*['\"]?\\s*refresh", RegexOptions.IgnoreCase))
      return true;

    // Base tag (changes relative URL resolution)
    if (Regex.IsMatch(content, "<\\s*base\\b", RegexOptions.IgnoreCase))
      return true;

    // Link tag with import or stylesheet (can load external malicious content)
    if (Regex.IsMatch(content, "<\\s*link[^>]*?(rel\\s*=\\s*['\"]?\\s*(import|stylesheet)|href\\s*=\\s*['\"]?\\s*javascript:)", RegexOptions.IgnoreCase))
      return true;

    // Style expressions (IE-specific XSS)
    if (Regex.IsMatch(content, "expression\\s*\\(", RegexOptions.IgnoreCase))
      return true;

    // url() in style with javascript
    if (Regex.IsMatch(content, "url\\s*\\(\\s*['\"]?\\s*(javascript|data):", RegexOptions.IgnoreCase))
      return true;

    // @import in style tags
    if (Regex.IsMatch(content, "@import\\s*['\"]?\\s*(javascript|data):", RegexOptions.IgnoreCase))
      return true;

    // behavior CSS property (IE-specific)
    if (Regex.IsMatch(content, "behavior\\s*:\\s*url", RegexOptions.IgnoreCase))
      return true;

    // -moz-binding (Firefox XSS)
    if (Regex.IsMatch(content, "-moz-binding\\s*:", RegexOptions.IgnoreCase))
      return true;

    // srcdoc attribute (can contain inline HTML with scripts)
    if (Regex.IsMatch(content, "\\bsrcdoc\\s*=", RegexOptions.IgnoreCase))
      return true;

    // Form tags (CSRF/phishing vectors)
    if (Regex.IsMatch(content, "<\\s*form\\b", RegexOptions.IgnoreCase))
      return true;

    // SVG with potential XSS vectors
    if (Regex.IsMatch(content, "<\\s*svg\\b", RegexOptions.IgnoreCase))
      return true;

    // foreignObject in SVG (can contain HTML)
    if (Regex.IsMatch(content, "<\\s*foreignObject\\b", RegexOptions.IgnoreCase))
      return true;

    // Math tags (MathML XSS)
    if (Regex.IsMatch(content, "<\\s*math\\b", RegexOptions.IgnoreCase))
      return true;

    // animate/set tags in SVG (can trigger events)
    if (Regex.IsMatch(content, "<\\s*(animate|set|animateTransform|animateMotion)\\b", RegexOptions.IgnoreCase))
      return true;

    // XML processing instructions
    if (Regex.IsMatch(content, "<\\?xml", RegexOptions.IgnoreCase))
      return true;

    // Import maps (can override module imports)
    if (Regex.IsMatch(content, "<\\s*script[^>]*type\\s*=\\s*['\"]?\\s*importmap", RegexOptions.IgnoreCase))
      return true;

    // Livescript (old Netscape, but still worth checking)
    if (Regex.IsMatch(content, "livescript\\s*:", RegexOptions.IgnoreCase))
      return true;

    // XLink href in SVG (can execute JavaScript)
    if (Regex.IsMatch(content, "xlink:href\\s*=\\s*['\"]?\\s*javascript:", RegexOptions.IgnoreCase))
      return true;

    // FSCommand (Flash XSS, deprecated but still exists)
    if (Regex.IsMatch(content, "fscommand\\s*:", RegexOptions.IgnoreCase))
      return true;

    // Suspicious attributes that can load external content
    if (Regex.IsMatch(content, "\\b(dynsrc|lowsrc)\\s*=", RegexOptions.IgnoreCase))
      return true;

    // HTML5 formaction attribute (can override form action)
    if (Regex.IsMatch(content, "\\bformaction\\s*=", RegexOptions.IgnoreCase))
      return true;

    // Unusually large payload (potential DoS or obfuscation)
    if (content.Length > 200_000)
      return true;

    // Excessive nesting (potential billion laughs attack or DoS)
    if (CountNestedTags(content) > 100)
      return true;

    // Multiple script-like patterns (indicates obfuscation attempt)
    int suspiciousPatternCount = 0;
    if (content.Contains("eval(", StringComparison.OrdinalIgnoreCase)) suspiciousPatternCount++;
    if (content.Contains("fromCharCode", StringComparison.OrdinalIgnoreCase)) suspiciousPatternCount++;
    if (content.Contains("document.write", StringComparison.OrdinalIgnoreCase)) suspiciousPatternCount++;
    if (content.Contains("innerHTML", StringComparison.OrdinalIgnoreCase)) suspiciousPatternCount++;
    if (content.Contains("outerHTML", StringComparison.OrdinalIgnoreCase)) suspiciousPatternCount++;
    if (suspiciousPatternCount >= 2) return true;

    // Windows-specific protocols that can execute files
    if (Regex.IsMatch(content, "\\b(file|smb|ftp)://", RegexOptions.IgnoreCase))
      return true;

    return false;
  }

  private static string DecodeHtmlEntities(string html)
  {
    try
    {
      // Decode HTML entities multiple times to catch double-encoding
      string decoded = html;
      string previous;
      int iterations = 0;

      do
      {
        previous = decoded;
        decoded = HttpUtility.HtmlDecode(decoded);
        iterations++;
      } while (decoded != previous && iterations < 5);

      return decoded;
    }
    catch
    {
      return html;
    }
  }

  private static string NormalizeWhitespace(string content)
  {
    // Replace tabs, newlines, carriage returns, and null bytes with spaces
    return Regex.Replace(content, @"[\t\n\r\0\u200B\uFEFF]", " ", RegexOptions.None);
  }

  private static int CountNestedTags(string content)
  {
    int maxDepth = 0;
    int currentDepth = 0;

    foreach (Match match in Regex.Matches(content, "</?[a-z][a-z0-9]*", RegexOptions.IgnoreCase))
    {
      if (!match.Value.StartsWith("</"))
      {
        currentDepth++;
        maxDepth = Math.Max(maxDepth, currentDepth);
      }
      else currentDepth = Math.Max(0, currentDepth - 1);
    }
    return maxDepth;
  }
}