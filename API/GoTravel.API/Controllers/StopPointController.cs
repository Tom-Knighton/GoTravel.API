using System.ComponentModel.DataAnnotations;
using System.Net;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

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
    [Produces(typeof(ICollection<StopPointBaseDto>))]
    [ProducesResponseType(typeof(ICollection<StopPointBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchByName(string searchQuery, [FromQuery] List<string> filterLineMode, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsByNameAsync(searchQuery, filterLineMode, maxResults, ct);
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
    [Produces(typeof(ICollection<StopPointBaseDto>))]
    [ProducesResponseType(typeof(ICollection<StopPointBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchAroundPoint(float lat, float lon, [FromQuery] List<string> filterLineMode, [Range(1, 10000)] int radius = 850, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsAroundPointAsync(lat, lon, filterLineMode, radius, maxResults, ct);
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