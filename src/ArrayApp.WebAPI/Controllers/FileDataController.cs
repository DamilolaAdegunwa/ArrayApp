using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.FileAggregate;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FileDataController : ControllerBase
{
    private readonly IFileDataService _fileService;
    private readonly ILogger<FileDataController> _logger;

    public FileDataController(IFileDataService fileService, ILogger<FileDataController> logger)
    {
        _fileService = fileService;
        _logger = logger;
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
            _logger.LogError(ex, "Error while uploading file");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = "An error occurred while uploading the file.",
                Description = "File upload processing failed. Please check file properties and try again.",
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
            _logger.LogError(ex, "Error while retrieving file {FileId}", fileId);
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = "An error occurred while retrieving the file.",
                Description = "File retrieval failed.",
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
            _logger.LogError(ex, "Error while deleting file {FileId}", fileId);
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = "An error occurred while deleting the file.",
                Description = "File deletion failed.",
            });
        }
    }
}
