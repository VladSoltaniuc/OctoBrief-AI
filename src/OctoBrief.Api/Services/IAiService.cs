using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IAiService
{
  Task<string> GenerateCards(List<WebsiteData> websiteData);
}