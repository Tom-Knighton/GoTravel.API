using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class StopPointController: ControllerBase
{
    private readonly IStopPointService _stopPointService;
    private readonly IArrivalsService _arrivalsService;

    public StopPointController(IStopPointService stopPointService, IArrivalsService arrivals)
    {
        _stopPointService = stopPointService;
        _arrivalsService = arrivals;
    }

    [HttpGet]
    [Route("Search/{searchQuery}")]
    [Produces(typeof(ICollection<StopPointBaseDto>))]
    [ProducesResponseType(typeof(ICollection<StopPointBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchByName(string searchQuery, [FromQuery] List<string> hiddenLineModes, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsByNameAsync(searchQuery, hiddenLineModes, maxResults, ct);
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
    public async Task<IActionResult> SearchAroundPoint(float lat, float lon, [FromQuery] List<string> hiddenLineModes, [Range(1, 10000)] int radius = 850, [Range(1, 255)] int maxResults = 25, CancellationToken ct = default)
    {
        try
        {
            var results = await _stopPointService.GetStopPointsAroundPointAsync(lat, lon, hiddenLineModes, radius, maxResults, ct);
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
    [Route("{stopId}/Arrivals")]
    [Produces(typeof(StopPointArrivalsDto))]
    public async Task<IActionResult> GetArrivalsForStop(string stopId, bool includeChildrenAndHubs = false, CancellationToken ct = default)
    {
        try
        {
            var results = await _arrivalsService.GetArrivalsForStopPointAsync(stopId, includeChildrenAndHubs, ct);
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
    [Route("{stopId}/Info")]
    [Produces(typeof(StopPointInformationDto))]
    public async Task<IActionResult> GetInformationForStop(string stopId, bool useParentOrHub = false, CancellationToken ct = default)
    {
        try
        {
            var result = await _stopPointService.GetStopPointInformation(stopId, useParentOrHub, ct);
            return Ok(result);
        }
        catch (NoStopPointException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}