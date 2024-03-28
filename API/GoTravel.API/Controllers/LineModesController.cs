using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LineModesController: ControllerBase
{
    private readonly ILineModeService _lineModeService;
    private readonly ILogger<LineModesController> _log;
    
    public LineModesController(ILineModeService service, ILogger<LineModesController> log)
    {
        _lineModeService = service;
        _log = log;
    }
    
    [HttpGet]
    [Produces(typeof(IEnumerable<LineModeSearchResult>))]
    public async Task<IActionResult> GetLineModes(float? lat, float? lon, CancellationToken ct = default)
    {
        if (lat is not null && lon is null || lat is null && lon is not null)
        {
            return BadRequest(new { error = "Both latitude and longitude must be provided, or neither provided." });
        }

        try
        {
            var results = await _lineModeService.ListAsync(lat, lon, ct);
            return Ok(results);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpGet]
    [Route("AreaFromCoordinates/{lat:float}/{lon:float}")]
    [Produces(typeof(string))]
    public async Task<IActionResult> GetLineModeAreaFromCoordinates(float lat, float lon, CancellationToken ct = default)
    {
        try
        {
            var results = await _lineModeService.GetAreaNameFromCoordinates(lat, lon, ct);
            return Ok(results);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpGet]
    [Route("Search/{query}")]
    [Produces(typeof(ICollection<UserDto>))]
    public async Task<IActionResult> SearchLineModes([Required] string query, int maxResults = 50, CancellationToken ct = default)
    {
        try
        {
            var results = await _lineModeService.SearchByName(query, maxResults, ct);
            return Ok(results);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Failed to search line modes with query: {Query}", query);
            return StatusCode(500, new { error = e.Message });
        }
    }
}