#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAgentsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IAIAgentService _aiAgentService;

    public AIAgentsController(ISender mediator, IAIAgentService aiAgentService)
    {
        _mediator = mediator;
        _aiAgentService = aiAgentService;
    }

    [HttpPost("invoke")]
    public async Task<ActionResult<AIAgentInsightDto>> InvokeAgent([FromBody] InvokeAIAgentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("insights/{ideaId}")]
    public async Task<ActionResult<List<AIAgentInsightDto>>> GetInsights(int ideaId, [FromQuery] int? sessionId)
    {
        var insights = await _mediator.Send(new GetAIAgentInsightsQuery(ideaId, sessionId));
        return Ok(insights);
    }

    [HttpPut("insights/{insightId}/pin")]
    public async Task<ActionResult<bool>> PinInsight(int insightId)
    {
        var isPinned = await _mediator.Send(new PinAIAgentInsightCommand(insightId));
        return Ok(isPinned);
    }

    [HttpPost("triage/{ideaId}")]
    public async Task<ActionResult<IdeaTriageResultDto>> TriageIdea(int ideaId)
    {
        var result = await _aiAgentService.TriageIdeaAsync(ideaId);
        return Ok(result);
    }

    [HttpPost("swot/{ideaId}")]
    public async Task<ActionResult<IdeaSwotAnalysisDto>> GenerateSwot(int ideaId)
    {
        var swot = await _aiAgentService.GenerateSwotAnalysisAsync(ideaId);
        return Ok(swot);
    }
}
