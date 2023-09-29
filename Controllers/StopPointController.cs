using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> SearchByName(string searchQuery, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsByNameAsync(searchQuery, maxResults, ct);
            return Ok(results);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpGet]
    [Route("Search/Around/{lat:float}/{lon:float}")]
    public async Task<IActionResult> SearchAroundPoint(float lat, float lon, [Range(1, 10000)] int radius = 850, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsAroundPointAsync(lat, lon, radius, maxResults, ct);
            return Ok(results);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}