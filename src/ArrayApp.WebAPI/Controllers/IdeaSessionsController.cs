using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeaSessionsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IAIAgentService _aiAgentService;
    private readonly IReputationService _reputationService;

    public IdeaSessionsController(IApplicationDbContext context, IAIAgentService aiAgentService, IReputationService reputationService)
    {
        _context = context;
        _aiAgentService = aiAgentService;
        _reputationService = reputationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<IdeaSessionDto>>> GetSessions([FromQuery] int? ideaId)
    {
        var query = _context.Sessions
            .Include(s => s.PrimaryIdea)
            .Include(s => s.Attendees)
            .Include(s => s.CanvasNodes)
            .Include(s => s.Decisions)
            .Include(s => s.ExtractedActions)
            .Include(s => s.AiInsights)
            .AsNoTracking();

        if (ideaId.HasValue)
        {
            query = query.Where(s => s.PrimaryIdeaId == ideaId.Value);
        }

        var sessions = await query.OrderByDescending(s => s.ScheduledStartTime).ToListAsync();
        return Ok(sessions.Select(MapToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdeaSessionDto>> GetSessionById(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.PrimaryIdea)
            .Include(s => s.Attendees)
            .Include(s => s.CanvasNodes)
            .Include(s => s.Decisions)
            .Include(s => s.ExtractedActions)
            .Include(s => s.AiInsights)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return NotFound();
        return Ok(MapToDto(session));
    }

    [HttpPost]
    public async Task<ActionResult<IdeaSessionDto>> CreateSession([FromBody] CreateSessionDto dto)
    {
        var idea = await _context.Ideas.FindAsync(dto.IdeaId);
        if (idea == null) return NotFound("Idea not found");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        var userName = User.Identity?.Name ?? "Host";

        var session = new Session
        {
            PrimaryIdeaId = dto.IdeaId,
            Name = dto.Name,
            Description = dto.Description,
            SessionType = dto.SessionType,
            SessionStatus = SessionStatus.Scheduled,
            ScheduledStartTime = dto.ScheduledStartTime,
            Duration = TimeSpan.FromMinutes(dto.DurationMinutes),
            MeetingUrl = $"https://meet.arrayapp.io/session-{Guid.NewGuid().ToString().Substring(0, 8)}",
            AgendaNotes = dto.AgendaNotes
        };

        // Add creator as Host
        session.Attendees.Add(new SessionParticipant
        {
            UserId = userId,
            DisplayName = $"{userName} (Host)",
            Role = ParticipantRole.Creator,
            IsHost = true
        });

        // Add invited AI Agents if any
        if (dto.InviteAiAgents != null)
        {
            foreach (var agentType in dto.InviteAiAgents)
            {
                session.Attendees.Add(new SessionParticipant
                {
                    UserId = $"ai-{agentType.ToString().ToLower()}",
                    DisplayName = $"{agentType} Bot (AI)",
                    Role = ParticipantRole.Researcher,
                    IsAiAgent = true,
                    AiAgentType = agentType.ToString()
                });
            }
        }

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(default);

        await _reputationService.AwardPointsAsync(userId, 30, "Scheduled a new collaborative idea session");

        return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, MapToDto(session));
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinSession(int id, [FromBody] JoinSessionDto dto)
    {
        var session = await _context.Sessions.Include(s => s.Attendees).FirstOrDefaultAsync(s => s.Id == id);
        if (session == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? $"user-{Guid.NewGuid().ToString().Substring(0, 6)}";
        var displayName = dto.DisplayName ?? $"Contributor ({dto.Role})";

        var attendee = session.Attendees.FirstOrDefault(a => a.UserId == userId);
        if (attendee == null)
        {
            session.Attendees.Add(new SessionParticipant
            {
                SessionId = id,
                UserId = userId,
                DisplayName = displayName,
                Role = dto.Role,
                JoinedAt = DateTime.UtcNow
            });
            await _reputationService.AwardPointsAsync(userId, 15, "Joined a live idea session");
        }
        else
        {
            attendee.Role = dto.Role;
        }

        if (session.SessionStatus == SessionStatus.Scheduled)
        {
            session.SessionStatus = SessionStatus.Live;
            session.ActualStartTime = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(default);
        return Ok(new { success = true, sessionId = id, role = dto.Role.ToString() });
    }

    [HttpPost("{id}/canvas")]
    public async Task<ActionResult<IdeaCanvasNodeDto>> AddOrUpdateCanvasNode(int id, [FromBody] UpdateCanvasNodeDto dto)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return NotFound();

        IdeaCanvasNode node;
        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            node = await _context.CanvasNodes.FindAsync(dto.Id.Value) ?? new IdeaCanvasNode();
            node.Content = dto.Content;
            node.PosX = dto.PosX;
            node.PosY = dto.PosY;
            node.ColorHex = dto.ColorHex;
            node.VotesCount = dto.VotesCount;
        }
        else
        {
            node = new IdeaCanvasNode
            {
                IdeaId = session.PrimaryIdeaId ?? dto.IdeaId,
                SessionId = id,
                NodeType = dto.NodeType,
                Content = dto.Content,
                PosX = dto.PosX,
                PosY = dto.PosY,
                ColorHex = dto.ColorHex,
                VotesCount = 1,
                AuthorName = dto.AuthorName ?? "Participant"
            };
            _context.CanvasNodes.Add(node);
        }

        await _context.SaveChangesAsync(default);

        return Ok(new IdeaCanvasNodeDto
        {
            Id = node.Id,
            IdeaId = node.IdeaId,
            SessionId = node.SessionId,
            NodeType = node.NodeType,
            Content = node.Content,
            PosX = node.PosX,
            PosY = node.PosY,
            ColorHex = node.ColorHex,
            VotesCount = node.VotesCount,
            AuthorName = node.AuthorName
        });
    }

    [HttpPost("{id}/extract-outcomes")]
    public async Task<IActionResult> ExtractSessionOutcomes(int id, [FromBody] ExtractSessionOutcomesDto dto)
    {
        var session = await _context.Sessions
            .Include(s => s.PrimaryIdea)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return NotFound();

        session.SharedNotes = dto.SessionNotes;
        session.SessionStatus = SessionStatus.Completed;
        session.ActualEndTime = DateTime.UtcNow;

        if (dto.Decisions != null)
        {
            foreach (var dec in dto.Decisions)
            {
                var decision = new IdeaDecision
                {
                    IdeaId = session.PrimaryIdeaId ?? dec.IdeaId,
                    SessionId = id,
                    Summary = dec.Summary,
                    Rationale = dec.Rationale,
                    Context = dec.Context,
                    DecidedAt = DateTime.UtcNow
                };
                _context.Decisions.Add(decision);
            }
        }

        if (dto.Actions != null)
        {
            foreach (var act in dto.Actions)
            {
                var action = new IdeaAction
                {
                    IdeaId = session.PrimaryIdeaId ?? act.IdeaId,
                    SessionId = id,
                    Title = act.Title,
                    Description = act.Description,
                    Priority = act.Priority,
                    DueDate = act.DueDate,
                    Status = ActionItemStatus.Todo,
                    ExternalSystem = act.ExternalSystem
                };
                _context.Actions.Add(action);
            }
        }

        if (dto.GenerateAiSummary)
        {
            await _aiAgentService.GenerateSessionSummaryAsync(id);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        await _reputationService.AwardPointsAsync(userId, 50, "Extracted decisions and action points from a completed session");

        await _context.SaveChangesAsync(default);
        return Ok(new { success = true, sessionId = id, summary = session.AiSummary });
    }

    private static IdeaSessionDto MapToDto(Session session)
    {
        return new IdeaSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            SessionType = session.SessionType,
            SessionStatus = session.SessionStatus,
            ScheduledStartTime = session.ScheduledStartTime,
            ActualStartTime = session.ActualStartTime,
            ActualEndTime = session.ActualEndTime,
            Duration = session.Duration,
            MeetingUrl = session.MeetingUrl,
            AgendaNotes = session.AgendaNotes,
            SharedNotes = session.SharedNotes,
            AiSummary = session.AiSummary,
            PrimaryIdeaId = session.PrimaryIdeaId,
            PrimaryIdeaTitle = session.PrimaryIdea?.Title,
            Attendees = session.Attendees?.Select(a => new SessionParticipantDto
            {
                Id = a.Id,
                SessionId = a.SessionId,
                UserId = a.UserId,
                DisplayName = a.DisplayName,
                Role = a.Role,
                IsHost = a.IsHost,
                IsAiAgent = a.IsAiAgent,
                AiAgentType = a.AiAgentType,
                JoinedAt = a.JoinedAt
            }).ToList() ?? new(),
            CanvasNodes = session.CanvasNodes?.Select(c => new IdeaCanvasNodeDto
            {
                Id = c.Id,
                IdeaId = c.IdeaId,
                SessionId = c.SessionId,
                NodeType = c.NodeType,
                Content = c.Content,
                PosX = c.PosX,
                PosY = c.PosY,
                ColorHex = c.ColorHex,
                VotesCount = c.VotesCount,
                AuthorName = c.AuthorName
            }).ToList() ?? new(),
            Decisions = session.Decisions?.Select(d => new IdeaDecisionDto
            {
                Id = d.Id,
                IdeaId = d.IdeaId,
                SessionId = d.SessionId,
                Summary = d.Summary,
                Rationale = d.Rationale,
                Context = d.Context,
                DecidedAt = d.DecidedAt
            }).ToList() ?? new(),
            Actions = session.ExtractedActions?.Select(a => new IdeaActionDto
            {
                Id = a.Id,
                IdeaId = a.IdeaId,
                SessionId = a.SessionId,
                Title = a.Title,
                Description = a.Description,
                Priority = a.Priority,
                Status = a.Status,
                DueDate = a.DueDate,
                ExternalSystem = a.ExternalSystem,
                ExternalReferenceKey = a.ExternalReferenceKey,
                ExternalUrl = a.ExternalUrl
            }).ToList() ?? new()
        };
    }
}
