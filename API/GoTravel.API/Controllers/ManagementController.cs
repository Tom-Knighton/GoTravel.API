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

    public ManagementController(IStopPointService sps, ILogger<ManagementController> log)
    {
        _stopPointService = sps;
        _logger = log;
    }
    
    [Authorize("ManageStops")]
    [HttpGet]
    [Route("Stops/All")]
    [Produces(typeof(StopPointBaseDto[]))]
    public async Task<IActionResult> GetAllStopPoints(int results = 25, int startFrom = 0, CancellationToken ct = default)
    {
        try
        {
            var stops = await _stopPointService.RetrievePaginated(results, startFrom, ct);
            return Ok(stops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve stop points");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize("ManageStops")]
    [HttpGet]
    [Route("Stops/{stopId}/InfoKVs")]
    [Produces(typeof(Dictionary<string, string>))]
    public async Task<IActionResult> GetStopPointInfoKvs(string stopId, CancellationToken ct = default)
    {
        try
        {
            var infos = await _stopPointService.GetStopPointInfoKvs(stopId, ct);
            return Ok(infos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve stop points");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}