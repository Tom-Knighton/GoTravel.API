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
            if (maxResults < 1)
            {
                throw new ArgumentException("Max results must be greater than 0.");
            }
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
}