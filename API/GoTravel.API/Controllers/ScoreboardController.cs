using System.Collections;
using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ScoreboardController: ControllerBase
{
    private readonly IScoreboardService _scoreboardService;
    private readonly ILogger<ScoreboardController> _log;
    
    public ScoreboardController(IScoreboardService scoreboardService, ILogger<ScoreboardController> log)
    {
        _scoreboardService = scoreboardService;
        _log = log;
    }

    [HttpGet]
    [Route("User/{userId}")]
    [Produces(typeof(ICollection<ScoreboardDto>))]
    public async Task<IActionResult> GetScoreboardsForUser(string userId, CancellationToken ct = default)
    {
        try
        {
            var scoreboards = await _scoreboardService.GetScoreboardsForUser(userId, ct);
            return Ok(scoreboards);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve scoreboards for user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("{scoreboardId}")]
    [Produces(typeof(ScoreboardDto))]
    public async Task<IActionResult> GetScoreboard(string scoreboardId, CancellationToken ct = default)
    {
        return Ok();
    }

    [HttpGet]
    [Route("{scoreboardId}/Users")]
    [Produces(typeof(ICollection<ScoreboardUserDto>))]
    public async Task<IActionResult> GetScoreboardUsers([Required] int fromPosition = 1, int results = 10, CancellationToken ct = default)
    {
        return Ok();
    }
}