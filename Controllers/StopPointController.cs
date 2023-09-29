using System.Net;
using GoLondon.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoLondon.API.Controllers;

[ApiController]
[Route("[controller]")]
public class StopPointController: ControllerBase
{
    private readonly IStopPointService _stopPointService;

    public StopPointController(IStopPointService stopPointService)
    {
        _stopPointService = stopPointService;
    }

    [HttpGet]
    [Route("Search/{searchQuery}")]
    public async Task<IActionResult> SearchByName(string searchQuery, int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsByNameAsync(searchQuery, maxResults, ct);
            return Ok(results);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}