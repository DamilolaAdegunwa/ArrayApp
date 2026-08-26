using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeaActionsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IConnectorService _connectorService;
    private readonly IReputationService _reputationService;

    public IdeaActionsController(IApplicationDbContext context, IConnectorService connectorService, IReputationService reputationService)
    {
        _context = context;
        _connectorService = connectorService;
        _reputationService = reputationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<IdeaActionDto>>> GetActions([FromQuery] int? ideaId, [FromQuery] ActionItemStatus? status)
    {
        var query = _context.Actions
            .Include(a => a.Idea)
            .Include(a => a.OwnerUser)
            .AsNoTracking();

        if (ideaId.HasValue)
        {
            query = query.Where(a => a.IdeaId == ideaId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var actions = await query.OrderBy(a => a.DueDate).ToListAsync();

        return Ok(actions.Select(a => new IdeaActionDto
        {
            Id = a.Id,
            IdeaId = a.IdeaId,
            IdeaTitle = a.Idea?.Title,
            SessionId = a.SessionId,
            DecisionId = a.DecisionId,
            Title = a.Title,
            Description = a.Description,
            OwnerUserId = a.OwnerUserId,
            OwnerName = a.OwnerUser?.UserName ?? a.SupportingTeam ?? "Unassigned",
            SupportingTeam = a.SupportingTeam,
            Priority = a.Priority,
            Status = a.Status,
            DueDate = a.DueDate,
            CompletedAt = a.CompletedAt,
            Dependencies = a.Dependencies,
            ExternalSystem = a.ExternalSystem,
            ExternalReferenceKey = a.ExternalReferenceKey,
            ExternalUrl = a.ExternalUrl
        }).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<IdeaActionDto>> CreateAction([FromBody] CreateActionDto dto)
    {
        var idea = await _context.Ideas.FindAsync(dto.IdeaId);
        if (idea == null) return NotFound("Idea not found");

        var action = new IdeaAction
        {
            IdeaId = dto.IdeaId,
            SessionId = dto.SessionId,
            DecisionId = dto.DecisionId,
            Title = dto.Title,
            Description = dto.Description,
            OwnerUserId = dto.OwnerUserId,
            SupportingTeam = dto.SupportingTeam,
            Priority = dto.Priority,
            Status = ActionItemStatus.Todo,
            DueDate = dto.DueDate,
            Dependencies = dto.Dependencies,
            ExternalSystem = dto.ExternalSystem
        };

        _context.Actions.Add(action);

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = dto.IdeaId,
            ActorName = "Action Planner",
            ActorRole = "Actioner",
            ActionPerformed = "ActionCreated",
            Details = $"Action '{action.Title}' scheduled.",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(default);

        // If an external system was designated, automatically sync
        if (!string.IsNullOrWhiteSpace(dto.ExternalSystem) && Enum.TryParse<ConnectorType>(dto.ExternalSystem, out var connType))
        {
            await _connectorService.SyncActionAsync(action.Id, connType);
        }

        return Ok(new IdeaActionDto
        {
            Id = action.Id,
            IdeaId = action.IdeaId,
            IdeaTitle = idea.Title,
            Title = action.Title,
            Description = action.Description,
            Priority = action.Priority,
            Status = action.Status,
            DueDate = action.DueDate,
            ExternalSystem = action.ExternalSystem,
            ExternalReferenceKey = action.ExternalReferenceKey,
            ExternalUrl = action.ExternalUrl
        });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateActionStatusDto dto)
    {
        var action = await _context.Actions.Include(a => a.Idea).FirstOrDefaultAsync(a => a.Id == id);
        if (action == null) return NotFound();

        action.Status = dto.NewStatus;
        if (dto.NewStatus == ActionItemStatus.Done)
        {
            action.CompletedAt = DateTime.UtcNow;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
            await _reputationService.RecordActionCompletionAsync(userId, id);
        }

        await _context.SaveChangesAsync(default);
        return NoContent();
    }

    [HttpPost("{id}/sync")]
    public async Task<ActionResult<ConnectorSyncLogDto>> SyncAction(int id, [FromBody] SyncActionToConnectorDto dto)
    {
        var syncResult = await _connectorService.SyncActionAsync(id, dto.ConnectorType);
        return Ok(syncResult);
    }
}
