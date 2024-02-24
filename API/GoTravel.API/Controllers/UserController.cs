using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GoTravel.API.Domain.Exceptions;
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

    public UserController(IUserService users)
    {
        _userService = users;
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
        catch
        {
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
        catch
        {

            //TODO: Log
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
        catch (UserNotFoundException)
        {
            return Unauthorized();
        }
        catch
        {
            //TODO: Log
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{username}/updateDetails")]
    [Produces(typeof(bool))]
    public async Task<IActionResult> UpdateUserDetails(string username, [FromBody] UpdateUserDetailsDto dto, CancellationToken ct = default)
    {
        try
        {
            await _userService.ThrowIfUserOperatingOnOtherUser(username, ct);
            if (await _userService.UpdateUserDetails(username, dto, ct))
            {
                return Ok(true);
            }

            return BadRequest("Username is already taken");
        }
        catch (WrongUserException)
        {
            return Unauthorized();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            //TODO: Log
            Console.WriteLine(ex.Message);
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
            return Unauthorized();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch
        {
            //TODO: Log
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}