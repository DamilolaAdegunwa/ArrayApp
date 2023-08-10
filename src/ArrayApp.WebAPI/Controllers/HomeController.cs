using System.Diagnostics;
using ArrayApp.Application.Common.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HomeController : ApiControllerBase // : ControllerBase
{
    //private readonly IHomeService _homeService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(/*IHomeService homeService, */ILogger<HomeController> logger)
    {
        //_homeService = homeService;
        _logger = logger;
    }

    //[HttpGet("feed")]
    //[ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    //public async Task<IActionResult> GetFeed()
    //{
    //    try
    //    {
    //        // Call the service to get the main home feed
    //        var result = await _homeService.GetMainHomeFeed();

    //        return Ok(new ApiResponse<string>
    //        {
    //            Code = SystemCodes.Successful,
    //            Data = result,
    //            Description = "Main home feed retrieved successfully",
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error while retrieving main home feed");
    //        return BadRequest(new ApiResponse<string>
    //        {
    //            Code = SystemCodes.Failed,
    //            Data = ex.Message,
    //            Description = ex.StackTrace,
    //        });
    //    }
    //}
}
