#pragma warning disable
#pragma info disable
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LiveMeetingController : ControllerBase
{
    private readonly ISender _mediator;

    public LiveMeetingController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("token/{sessionId}")]
    public async Task<ActionResult<MeetingRoomCredentialsDto>> GetRoomToken(int sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-anonymous";
        var displayName = User.Identity?.Name ?? "Participant";

        var credentials = await _mediator.Send(new GenerateMeetingRoomTokenQuery(sessionId, userId, displayName));
        return Ok(credentials);
    }

    [HttpPost("diarization/extract")]
    public async Task<ActionResult<ExtractedSpeechOutcomesResultDto>> ExtractSpeechActions([FromBody] ExtractSpeechActionsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
