using OctoBrief.Api.Models;

namespace OctoBrief.Api.Services;

public interface IBriefService
{
  Task<BriefDto> GetBriefAsync(string topic, string country);
}
