using GoTravel.API.Domain.Services;
using GoTravel.Standard.Models.Journeys;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class JourneyController: ControllerBase
{

    private IJourneyService _journeyService;

    public JourneyController(IJourneyService journeyService)
    {
        _journeyService = journeyService;
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> GetJourneyOptions([FromBody] JourneyRequest request, CancellationToken ct = default)
    {
        try
        {
            var result = await _journeyService.GetJourneys(request, ct);

            return Ok(result);
        }
        catch (Exception ex)
        {
            //TODO: Log
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}