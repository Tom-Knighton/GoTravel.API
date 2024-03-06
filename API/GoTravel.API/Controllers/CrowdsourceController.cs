using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.ML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTravel.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CrowdsourceController: ControllerBase
{

    // private readonly ICrowdsourceService _crowdsource;

    // public CrowdsourceController(ICrowdsourceService service)
    // {
    //     _crowdsource = service;
    // }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Test(CancellationToken ct = default)
    {
        var list = CrowdsourceSimilarity.GroupEmbeddings(0.6);
        return Ok();
    }
}