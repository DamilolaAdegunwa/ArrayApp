using System.Net;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdvertController : ControllerBase
{
    private readonly IAdvertService _advertService;

    public AdvertController(IAdvertService advertService)
    {
        _advertService = advertService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<AdvertDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAdvert(AdvertCreateDto advertCreateDto)
    {
        try
        {
            var result = await _advertService.CreateAdvertAsync(advertCreateDto);
            return Ok(new ApiResponse<AdvertDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Advert created successfully",
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

    [HttpGet("get/{advertId}")]
    [ProducesResponseType(typeof(ApiResponse<AdvertDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAdvertById(int advertId)
    {
        try
        {
            var result = await _advertService.GetAdvertByIdAsync(advertId);
            return Ok(new ApiResponse<AdvertDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Advert retrieved successfully",
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AdvertDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllAdverts()
    {
        try
        {
            var result = await _advertService.GetAllAdvertsAsync();
            return Ok(new ApiResponse<IEnumerable<AdvertDto>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "All adverts retrieved successfully",
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
}
