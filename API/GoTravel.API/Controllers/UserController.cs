using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _log;
    
    public UserController(IUserService users, ILogger<UserController> log)
    {
        _userService = users;
        _log = log;
    }

    [Authorize(AuthenticationSchemes = "Auth0Only")]
    [HttpPost]
    [Route("webhook/postSignUp")]
    public async Task<IActionResult> PostSignUp([FromBody] AuthUserSignup dto, CancellationToken ct = default)
    {
        try
        {
            await _userService.CreateUser(dto, ct);
            return Ok();
        }
        catch(Exception ex)
        {
            _log.LogError(ex, "Fatal error on user sign up");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(AuthenticationSchemes = "Auth0Only")]
    [HttpGet]
    [Route("webhook/isValid/{query}")]
    public async Task<IActionResult> IsUsernameValid(string query, CancellationToken ct = default)
    {
        try
        {
            await _userService.GetUserInfoByIdOrUsername(query, ct);
            return Ok(false);
        }
        catch (UserNotFoundException)
        {
            return Ok(true);
        }
        catch
        {
            return Ok(false);
        }
    }
    

    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetUserById(string username, CancellationToken ct = default)
    {
        try
        {
            return Ok(await _userService.GetUserInfoByIdOrUsername(username, ct));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch(Exception ex)
        {
            _log.LogError(ex, "Error on retrieving user by id: {Id}", username);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct = default)
    {
        try
        {
            return Ok(await _userService.GetCurrentUserInfo(ct));
        }
        catch (UserNotFoundException ex)
        {
            _log.LogWarning(ex, "/me failed as user was not found in DB");
            return Unauthorized();
        }
        catch(Exception ex)
        {
            _log.LogError(ex, "Failed to retrieve current user from jwt despite being authenticated");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{username}/updateDetails")]
    [Produces(typeof(bool))]
    public async Task<IActionResult> UpdateUserDetails(string username, [FromBody] UpdateUserDetailsCommand command, CancellationToken ct = default)
    {
        try
        {
            await _userService.ThrowIfUserOperatingOnOtherUser(username, ct);
            if (await _userService.UpdateUserDetails(username, command, ct))
            {
                return Ok(true);
            }

            return BadRequest("Username is already taken");
        }
        catch (WrongUserException)
        {
            _log.LogWarning("{Id} tried to operate on {Username}, which is a different user", HttpContext.User.CurrentUserId(), username);
            return Unauthorized();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to update user details for {Username} with command: {@Command}", username, command);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpPut]
    [Route("{username}/updateProfilePicture")]
    [Produces(typeof(bool))]
    public async Task<IActionResult> UpdateUserProfilePicture(string username, [Required] IFormFile picture, CancellationToken ct = default)
    {
        try
        {
            await _userService.ThrowIfUserOperatingOnOtherUser(username, ct);
            var success = await _userService.UpdateProfilePictureUrl(username, picture, ct);
            return Ok(success);
        }
        catch (WrongUserException)
        {
            _log.LogWarning("{Id} tried to update profile picture for {Username}, which is a different user", HttpContext.User.CurrentUserId(), username);
            return Unauthorized();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to update a user's profile picture");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("Search/{query}")]
    [Produces(typeof(ICollection<UserDto>))]
    public async Task<IActionResult> SearchUsers(string query, [Range(1, 125)] int maxResults, CancellationToken ct = default)
    {
        try
        {
            var results = await _userService.SearchUsers(query, maxResults, ct);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to search for users with query: {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}