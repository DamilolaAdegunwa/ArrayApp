using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAgentsController : ControllerBase
{
    private readonly IAIAgentService _aiAgentService;
    private readonly IApplicationDbContext _context;

    public AIAgentsController(IAIAgentService aiAgentService, IApplicationDbContext context)
    {
        _aiAgentService = aiAgentService;
        _context = context;
    }

    [HttpGet("insights/{ideaId}")]
    public async Task<ActionResult<List<AIAgentInsightDto>>> GetInsights(int ideaId)
    {
        var insights = await _context.AIAgentInsights
            .Where(i => i.IdeaId == ideaId)
            .OrderByDescending(i => i.GeneratedAt)
            .Select(i => new AIAgentInsightDto
            {
                Id = i.Id,
                IdeaId = i.IdeaId,
                SessionId = i.SessionId,
                AgentType = i.AgentType,
                AgentName = i.AgentName,
                Title = i.Title,
                Summary = i.Summary,
                FullContent = i.FullContent,
                ConfidenceScore = i.ConfidenceScore,
                IsPinned = i.IsPinned,
                GeneratedAt = i.GeneratedAt
            })
            .ToListAsync();

        return Ok(insights);
    }

    [HttpPost("invoke")]
    public async Task<ActionResult<AIAgentInsightDto>> InvokeAgent([FromBody] InvokeAIAgentDto dto)
    {
        var insight = await _aiAgentService.RunAgentAnalysisAsync(dto.IdeaId, dto.AgentType, dto.CustomPrompt, dto.SessionId);
        return Ok(insight);
    }

    [HttpPost("session-summary/{sessionId}")]
    public async Task<ActionResult<string>> GenerateSessionSummary(int sessionId)
    {
        var summary = await _aiAgentService.GenerateSessionSummaryAsync(sessionId);
        return Ok(new { sessionId, summary });
    }

    [HttpPost("action-breakdown")]
    public async Task<ActionResult<List<CreateActionDto>>> GenerateActionBreakdown([FromQuery] int ideaId, [FromQuery] string decisionSummary)
    {
        var actions = await _aiAgentService.GenerateActionBreakdownAsync(ideaId, decisionSummary);
        return Ok(actions);
    }

    [HttpPost("mentor-question")]
    public async Task<ActionResult<string>> AskMentor([FromQuery] int ideaId, [FromQuery] string question, [FromQuery] string userRole)
    {
        var answer = await _aiAgentService.AnswerMentorQuestionAsync(ideaId, question, userRole);
        return Ok(new { ideaId, question, userRole, answer });
    }

    [HttpPost("detect-duplicates")]
    public async Task<ActionResult<List<DuplicateIdeaResultDto>>> DetectDuplicates([FromBody] DetectDuplicatesRequestDto dto)
    {
        var results = await _aiAgentService.DetectDuplicatesAsync(dto.Title, dto.Description);
        return Ok(results);
    }

    [HttpGet("cluster-ideas")]
    public async Task<ActionResult<List<IdeaClusterDto>>> ClusterIdeas()
    {
        var clusters = await _aiAgentService.ClusterIdeasAsync();
        return Ok(clusters);
    }

    [HttpPost("sessions/{sessionId}/synthesize-mindmap")]
    public async Task<ActionResult<SynthesizedMindMapDto>> SynthesizeMindMap(int sessionId)
    {
        var result = await _aiAgentService.SynthesizeMindMapAsync(sessionId);
        return Ok(result);
    }

    [HttpPost("triage-idea/{ideaId}")]
    public async Task<ActionResult<IdeaTriageResultDto>> TriageIdea(int ideaId)
    {
        var result = await _aiAgentService.TriageIdeaAsync(ideaId);
        return Ok(result);
    }

    [HttpGet("swot/{ideaId}")]
    public async Task<ActionResult<IdeaSwotAnalysisDto>> GetSwotAnalysis(int ideaId)
    {
        var result = await _aiAgentService.GenerateSwotAnalysisAsync(ideaId);
        return Ok(result);
    }

    [HttpPost("chat-with-ideabot")]
    public async Task<ActionResult<IdeaBotChatResponseDto>> ChatWithIdeaBot([FromBody] ChatWithIdeaBotRequestDto dto)
    {
        var result = await _aiAgentService.ChatWithIdeaBotAsync(dto.IdeaId, dto.Message, dto.IntentMode ?? "General");
        return Ok(result);
    }
}

public class ChatWithIdeaBotRequestDto
{
    public int IdeaId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? IntentMode { get; set; } // General, PatentSearch, GrantDraft
}

public class DetectDuplicatesRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
