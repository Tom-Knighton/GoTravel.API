using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CrowdsourceController: ControllerBase
{

    private readonly ICrowdsourceService _crowdsource;
    private readonly ILogger<CrowdsourceController> _log;

    public CrowdsourceController(ICrowdsourceService service, ILogger<CrowdsourceController> log)
    {
        _crowdsource = service;
        _log = log;
    }

    [HttpGet]
    [Route("Entity/{entityId}")]
    [AllowAnonymous]
    [Produces(typeof(ICollection<CrowdsourceInfoDto>))]
    public async Task<IActionResult> GetCrowdsourceResultsForEntity(string entityId, CancellationToken ct = default)
    {
        try
        {
            var dtos = await _crowdsource.GetCrowdsourceInfoForEntities(entityId, ct);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve crowdsource info for entity: {Entity}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("Entity/{entityId}")]
    [Authorize]
    public async Task<IActionResult> SubmitCrowdsourceForEntity(string entityId, [FromBody] AddCrowdsourceCommand command, CancellationToken ct = default)
    {
        try
        {
            await _crowdsource.SubmitCrowdsourceInfo(HttpContext.User.CurrentUserId(), entityId, command, ct);
            return Ok();
        }
        catch (BadModerationException ex)
        {
            _log.LogWarning(ex, "Bad moderation result for crowdsource request");
            return BadRequest("This message did not pass moderation, please try again and remember information must be concise and relevant.");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error submitting crowdsource info");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("{crowdsourceId}/vote")]
    [Authorize]
    public async Task<IActionResult> VoteOnCrowdsource(string crowdsourceId, [FromBody] CrowdsourceVoteCommand command, CancellationToken ct = default)
    {
        try
        {
            await _crowdsource.VoteOnCrowdsource(crowdsourceId, HttpContext.User.CurrentUserId(), command.voteType, ct);
            return Ok();
        }
        catch (NoCrowdsourceException ex)
        {
            _log.LogWarning(ex, "User tried to vote on non-existent crowdsource: {Crowdsource}, User: {User}", crowdsourceId, HttpContext.User.CurrentUserId());
            return NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error voting on crowdsource");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("{crowdsourceId}/report")]
    [Authorize]
    public async Task<IActionResult> ReportCrowdsource(string crowdsourceId, ReportCrowdsourceCommand command, CancellationToken ct = default)
    {
        try
        {
            await _crowdsource.ReportCrowdsource(crowdsourceId, HttpContext.User.CurrentUserId(), command, ct);
            _log.LogInformation("Crowdsource report made, crowdsource: {Id}, report: {@Command}", crowdsourceId, command);
            return Ok();
        }
        catch (NoCrowdsourceException ex)
        {
            _log.LogWarning(ex, "User tried to report non-existent crowdsource: {Crowdsource}, User: {User}", crowdsourceId, HttpContext.User.CurrentUserId());
            return NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error reporting crowdsource");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}