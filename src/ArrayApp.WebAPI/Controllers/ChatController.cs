using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<ChatDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateChat(ChatCreateDto chatCreateDto)
    {
        try
        {
            var result = await _chatService.CreateChatAsync(chatCreateDto);
            return Ok(new ApiResponse<ChatDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Chat created successfully",
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

    [HttpGet("get/{chatId}")]
    [ProducesResponseType(typeof(ApiResponse<ChatDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetChatById(int chatId)
    {
        try
        {
            var result = await _chatService.GetChatByIdAsync(chatId);
            return Ok(new ApiResponse<ChatDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Chat retrieved successfully",
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChatDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllChats()
    {
        try
        {
            var result = await _chatService.GetAllChatsAsync();
            return Ok(new ApiResponse<IEnumerable<ChatDto>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "All chats retrieved successfully",
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

    // Add more endpoints for other methods in the ChatService
}