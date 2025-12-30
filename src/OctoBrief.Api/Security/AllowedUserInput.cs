namespace OctoBrief.Api.Security;

public static class AllowedUserInput  // Prevents malicious input
{
  public static readonly HashSet<string> Countries = new(StringComparer.OrdinalIgnoreCase)
    {
        "romania", "usa", "uk", "canada", "germany", "france", "italy", "spain", "poland"
    };

  public static readonly HashSet<string> Topics = new(StringComparer.OrdinalIgnoreCase)
    {
        "technology", "science", "sports", "media", "health", "climate", "politics", "crypto", "gaming"
    };
}