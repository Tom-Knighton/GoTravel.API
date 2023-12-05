using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LineModesController: ControllerBase
{
    private readonly ILineModeService _lineModeService;

    public LineModesController(ILineModeService service)
    {
        _lineModeService = service;
    }
    
    [HttpGet]
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
}