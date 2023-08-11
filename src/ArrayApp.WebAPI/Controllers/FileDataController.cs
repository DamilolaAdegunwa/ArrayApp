using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.FileAggregate;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FileDataController : ControllerBase
{
    private readonly IFileDataService _fileService;

    public FileDataController(IFileDataService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponse<FileDataDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] FileDataUploadDto FileDataUploadDto)
    {
        try
        {
            var result = await _fileService.UploadFileAsync(FileDataUploadDto);
            return Ok(new ApiResponse<FileDataDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "File uploaded successfully",
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

    [HttpGet("get/{fileId}")]
    [ProducesResponseType(typeof(ApiResponse<FileDataDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFileById(int fileId)
    {
        try
        {
            var result = await _fileService.GetFileByIdAsync(fileId);
            return Ok(new ApiResponse<FileDataDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "File retrieved successfully",
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

    [HttpDelete("delete/{fileId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        try
        {
            await _fileService.DeleteFileAsync(fileId);
            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "File deleted successfully",
                Description = "File deleted successfully",
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
