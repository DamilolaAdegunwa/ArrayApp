using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<CommentDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateComment(CommentCreateDto commentCreateDto)
    {
        try
        {
            var result = await _commentService.CreateCommentAsync(commentCreateDto);
            return Ok(new ApiResponse<CommentDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Comment created successfully",
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

    [HttpGet("get/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse<CommentDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetCommentById(int commentId)
    {
        try
        {
            var result = await _commentService.GetCommentByIdAsync(commentId);
            return Ok(new ApiResponse<CommentDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Comment retrieved successfully",
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

    [HttpGet("getbyidea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CommentDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetCommentsByIdeaId(int ideaId)
    {
        try
        {
            var result = await _commentService.GetCommentsByIdeaIdAsync(ideaId);
            return Ok(new ApiResponse<IEnumerable<CommentDto>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Comments retrieved successfully",
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