using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Extensions;
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
        try
        {
            var scoreboard = await _scoreboardService.GetScoreboard(scoreboardId, ct);
            return Ok(scoreboard);
        }
        catch (ScoreboardNotFoundException ex)
        {
            _log.LogWarning(ex, "Could not find scoreboard");
            return NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve scoreboard information for scoreboard: {Id}", scoreboardId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("{scoreboardId}/Users")]
    [Produces(typeof(ICollection<ScoreboardUserDto>))]
    public async Task<IActionResult> GetScoreboardUsers(string scoreboardId, [Required] int fromPosition = 1, int results = 10, CancellationToken ct = default)
    {
        try
        {
            var users = await _scoreboardService.GetScoreboardUsers(scoreboardId, fromPosition, results, ct);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve users for scoreboard");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("win/{winId}/consumed")]
    public async Task<IActionResult> ConsumedFinalPosition(string winId, CancellationToken ct = default)
    {
        try
        {
            await _scoreboardService.SeenWin(winId, HttpContext.User.CurrentUserId(), ct);
            return Ok();
        }
        catch (WinNotFoundException ex)
        {
            _log.LogWarning(ex, "Could not find win '{WinId}'", winId);
            return NotFound();
        }
        catch (WrongUserException ex)
        {
            _log.LogWarning(ex, "User {User} tried to consume win that did not belong to them: {Win}", HttpContext.User.CurrentUserId(), winId);
            return BadRequest();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save Win seen for win: {Id}, {User}", winId, HttpContext.User.CurrentUserId());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("UnseenWins")]
    public async Task<IActionResult> GetUnseenWins(CancellationToken ct = default)
    {
        try
        {
            var wins = await _scoreboardService.GetUnseenWinsForUser(HttpContext.User.CurrentUserId(), ct);
            return Ok(wins);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve wins for user");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("SeenWins")]
    public async Task<IActionResult> GetSeenWins(CancellationToken ct = default)
    {
        try
        {
            var wins = await _scoreboardService.GetSeenWinsForUser(HttpContext.User.CurrentUserId(), ct);
            return Ok(wins);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve wins for user");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}