using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserFriendshipsController: ControllerBase
{
    private readonly IFriendshipsService _friendships;
    private ILogger<UserFriendshipsController> _log;

    public UserFriendshipsController(IFriendshipsService friendships, ILogger<UserFriendshipsController> log)
    {
        _friendships = friendships;
        _log = log;
    }

    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> UpdateRelationship([FromBody] SetRelationshipCommand command, CancellationToken ct = default)
    {
        try
        {
            var user = HttpContext.User.CurrentUserId();
            var success = await _friendships.UpdateRelationship(user, command, ct);
            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }
        catch (UserNotFoundException)
        {
            return BadRequest("Ensure the following user in the request exist");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "An error occurred updating a friendship");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("Respond")]
    public async Task<IActionResult> ApproveOrDenyRelationship([FromBody] ApproveRejectFollowCommand command, CancellationToken ct = default)
    {
        try
        {
            var success = await _friendships.ApproveRejectRelationship(HttpContext.User.CurrentUserId(), command, ct);
            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }
        catch (UserNotFoundException)
        {
            _log.LogWarning("Failed to find a user involved in a friendship approval");
            return BadRequest("Ensure the user has asked to follow this user");
        }
        catch (NoRelationshipException ex)
        {
            _log.LogWarning(ex, "A user tried to approve a relationship that didn't exist");
            return NotFound("No relationship between these users was found");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "An error occurred approving/rejecting a friendship");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    [Route("RemoveFollower")]
    public async Task<IActionResult> RemoveFollower(string followerId, CancellationToken ct = default)
    {
        try
        {
            var success = await _friendships.RemoveFollower(HttpContext.User.CurrentUserId(), followerId, ct);
            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }
        catch (UserNotFoundException)
        {
            _log.LogWarning("Failed to find a user involved in a friendship removal");
            return BadRequest("Ensure the user is following this user");
        }
        catch (NoRelationshipException ex)
        {
            _log.LogWarning(ex, "A user tried to remove a relationship that didn't exist");
            return NotFound("No relationship between these users was found");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "An error occurred removing a friendship");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}