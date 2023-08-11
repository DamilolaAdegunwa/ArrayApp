using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly IAppService _appService;

    public AppController(IAppService appService)
    {
        _appService = appService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<AppDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateApp(AppCreateDto appCreateDto)
    {
        try
        {
            var result = await _appService.CreateAppAsync(appCreateDto);
            return Ok(new ApiResponse<AppDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "App created successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("get/{appId}")]
    [ProducesResponseType(typeof(ApiResponse<AppDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAppById(int appId)
    {
        try
        {
            var result = await _appService.GetAppByIdAsync(appId);
            return Ok(new ApiResponse<AppDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "App retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AppDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllApps()
    {
        try
        {
            var result = await _appService.GetAllAppsAsync();
            return Ok(new ApiResponse<IEnumerable<AppDto>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "All apps retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    // Add more endpoints for other methods in the AppService
}
