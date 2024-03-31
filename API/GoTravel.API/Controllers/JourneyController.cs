using System.Collections;
using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using GoTravel.Standard.Models.Journeys;
using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class JourneyController: ControllerBase
{

    private IJourneyService _journeyService;
    private ITripService _tripService;
    private ILogger<JourneyController> _log;

    public JourneyController(IJourneyService journeyService, ITripService trip, ILogger<JourneyController> log)
    {
        _journeyService = journeyService;
        _tripService = trip;
        _log = log;
    }
    
    [HttpPost]
    [Route("")]
    [Produces(typeof(JourneyOptionsResultDto))]
    public async Task<IActionResult> GetJourneyOptions([FromBody] JourneyRequest request, CancellationToken ct = default)
    {
        try
        {
            var result = await _journeyService.GetJourneys(request, ct);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to make a journey request, request: {@Request}", request);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
 
    [HttpPost]
    [Route("SaveTrip")]
    [Authorize]
    [Idempotent(ExpiresInMilliseconds = 1_209_600_000, CacheOnlySuccessResponses = true)]
    public async Task<IActionResult> SaveUserTrip([Required(ErrorMessage = "IdempotencyKey header must be present")][FromHeader(Name = "IdempotencyKey")] string idempotencyKey, [FromBody] SaveUserTripCommand command, CancellationToken ct = default)
    {
        if (idempotencyKey is null)
        {
            return BadRequest("No key");
        }
        
        try
        {
            var dto = await _tripService.SaveUserTrip(command, HttpContext.User.CurrentUserId(), ct);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save user {UserId}'s trip", HttpContext.User.CurrentUserId());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("Trips")]
    [Authorize]
    public async Task<IActionResult> GetUserTrips(int results = 25, int startFrom = 1, CancellationToken ct = default)
    {
        try
        {
            var dtos = await _tripService.GetTripsForUser(HttpContext.User.CurrentUserId(), results, startFrom, ct);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve trips for user {UserId}, results = {Results}, start = {Start}", HttpContext.User.CurrentUserId(), results, startFrom);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}