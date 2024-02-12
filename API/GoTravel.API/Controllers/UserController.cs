using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService users)
    {
        _userService = users;
    }

    [Authorize(AuthenticationSchemes = "Auth0Only")]
    [HttpPost]
    [Route("postSignUp")]
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

    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetUserById(string username, CancellationToken ct = default)
    {
        try
        {
            return Ok(await _userService.GetUserInfoByIdOrUsername(username, ct));
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
        catch
        {
            
            //TODO: Log
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}