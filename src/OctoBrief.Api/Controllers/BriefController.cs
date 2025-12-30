using Microsoft.AspNetCore.Mvc;
using OctoBrief.Api.Models;
using OctoBrief.Api.Security;
using OctoBrief.Api.Services;

namespace OctoBrief.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BriefController : ControllerBase
{
  private readonly IBriefService _briefService;

  public BriefController(IBriefService briefService)
  {
    _briefService = briefService;
  }

  [HttpPost("preview")]
  public async Task<IActionResult> Preview([FromBody] GenerateBriefRequest request)
  {
    if (!AllowedUserInput.Countries.Contains(request.Country)) return BadRequest();
    if (!AllowedUserInput.Topics.Contains(request.Topic)) return BadRequest();

    var briefResult = await _briefService.GetBriefAsync(request.Topic, request.Country);
    if (!briefResult.Success) return StatusCode(500);
    return Ok(briefResult);
  }
}
