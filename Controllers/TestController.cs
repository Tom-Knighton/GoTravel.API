using GoLondon.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoLondon.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController: ControllerBase
{
    private readonly IStopPointService _stopService;

    public TestController(IStopPointService stopService)
    {
        _stopService = stopService;
    }
    
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Test()
    {
        return Ok("Hello World");
    }

    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> TestTest()
    {
        return Ok(await _stopService.GetStopPointsByNameAsync("STOP"));
    }
}