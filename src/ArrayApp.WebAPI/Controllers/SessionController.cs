using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<SessionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSession(SessionCreateDto sessionCreateDto)
    {
        var createdSession = await _sessionService.CreateSessionAsync(sessionCreateDto);
        return Ok(new ApiResponse<SessionDto>
        {
            Code = SystemCodes.Successful,
            Data = createdSession,
            Description = "Session created successfully",
        });
    }

    [HttpDelete("delete/{sessionId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteSession(int sessionId)
    {
        await _sessionService.DeleteSessionAsync(sessionId);
        return Ok(new ApiResponse<string>
        {
            Code = SystemCodes.Successful,
            Data = "Session deleted successfully",
            Description = $"Session with ID {sessionId} deleted",
        });
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllSessions()
    {
        var sessions = await _sessionService.GetAllSessionsAsync();
        return Ok(new ApiResponse<IEnumerable<SessionDto>>
        {
            Code = SystemCodes.Successful,
            Data = sessions,
            Description = "All sessions retrieved successfully",
        });
    }

    [HttpGet("past")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetPastSessions()
    {
        var pastSessions = await _sessionService.GetPastSessionsAsync();
        return Ok(new ApiResponse<IEnumerable<SessionDto>>
        {
            Code = SystemCodes.Successful,
            Data = pastSessions,
            Description = "Past sessions retrieved successfully",
        });
    }

    [HttpGet("get/{sessionId}")]
    [ProducesResponseType(typeof(ApiResponse<SessionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetSessionById(int sessionId)
    {
        var session = await _sessionService.GetSessionByIdAsync(sessionId);
        return Ok(new ApiResponse<SessionDto>
        {
            Code = SystemCodes.Successful,
            Data = session,
            Description = "Session retrieved successfully",
        });
    }

    // Add other methods based on the remaining service methods
}
