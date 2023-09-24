using Microsoft.AspNetCore.Mvc;

namespace GoLondon.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController: ControllerBase
{
    
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Test()
    {
        return Ok("Hello World");
    }
}