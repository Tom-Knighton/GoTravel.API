using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ManagementController: ControllerBase
{
    private readonly ILogger<ManagementController> _logger;
    private readonly IStopPointService _stopPointService;
    private readonly ICrowdsourceService _crowdsourceServide;

    public ManagementController(IStopPointService sps, ICrowdsourceService css, ILogger<ManagementController> log)
    {
        _stopPointService = sps;
        _crowdsourceServide = css;
        _logger = log;
    }
    
    [Authorize("ManageStops")]
    [HttpGet]
    [Route("Stops/All")]
    [Produces(typeof(ICollection<GTStopPoint>))]
    public async Task<IActionResult> GetAllStopPoints(string? query = null, int results = 25, int startFrom = 0, CancellationToken ct = default)
    {
        try
        {
            var stops = await _stopPointService.RetrievePaginated(query, results, startFrom, ct);
            return Ok(stops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve stop points");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize("ManageStops")]
    [HttpGet("Stops/{stopId}")]
    [Produces(typeof(ICollection<GTStopPoint>))]
    public async Task<IActionResult> GetStopPoint(string stopId, CancellationToken ct = default)
    {
        try
        {
            var stops = await _stopPointService.GetGTStop(stopId, ct);
            return Ok(stops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve stop point {Id}", stopId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize("ManageStops")]
    [HttpGet]
    [Route("Stops/{stopId}/InfoKVs")]
    [Produces(typeof(ICollection<GTStopPointInfoValue>))]
    public async Task<IActionResult> GetStopPointInfoKvs(string stopId, CancellationToken ct = default)
    {
        try
        {
            var infos = await _stopPointService.GetStopPointInfoKvs(stopId, ct);
            return Ok(infos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve stop point infos for {Id}", stopId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize("ManageStops")]
    [HttpGet]
    [Route("CrowdsourceSubmissions/{entityId}")]
    [Produces(typeof(ICollection<GTCrowdsourceInfo>))]
    public async Task<IActionResult> GetCrowdsources(string entityId, CancellationToken ct = default)
    {
        try
        {
            var infos = await _crowdsourceServide.GetGTSubmissions(entityId, ct);
            return Ok(infos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve crowdsource submissions for entity: {Id}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}