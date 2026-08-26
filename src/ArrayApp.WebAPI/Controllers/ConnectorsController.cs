using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectorsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IConnectorService _connectorService;

    public ConnectorsController(IApplicationDbContext context, IConnectorService connectorService)
    {
        _context = context;
        _connectorService = connectorService;
    }

    [HttpGet("{ideaId}")]
    public async Task<ActionResult<List<ConnectorConfigDto>>> GetConnectors(int ideaId)
    {
        var connectors = await _context.ConnectorConfigs
            .Where(c => c.IdeaId == ideaId)
            .Select(c => new ConnectorConfigDto
            {
                Id = c.Id,
                IdeaId = c.IdeaId,
                Type = c.Type,
                Name = c.Name,
                TargetEndpoint = c.TargetEndpoint,
                ProjectOrChannelKey = c.ProjectOrChannelKey,
                IsActive = c.IsActive,
                AutoSyncActions = c.AutoSyncActions,
                LastSyncTime = c.LastSyncTime
            })
            .ToListAsync();

        return Ok(connectors);
    }

    [HttpPost("configure")]
    public async Task<ActionResult<ConnectorConfigDto>> ConfigureConnector([FromBody] ConfigureConnectorDto dto)
    {
        var existing = await _context.ConnectorConfigs.FirstOrDefaultAsync(c => c.IdeaId == dto.IdeaId && c.Type == dto.Type);
        if (existing != null)
        {
            existing.Name = dto.Name;
            existing.TargetEndpoint = dto.TargetEndpoint;
            existing.ProjectOrChannelKey = dto.ProjectOrChannelKey;
            existing.AutoSyncActions = dto.AutoSyncActions;
            existing.IsActive = true;
        }
        else
        {
            existing = new ConnectorConfig
            {
                IdeaId = dto.IdeaId,
                Type = dto.Type,
                Name = dto.Name,
                TargetEndpoint = dto.TargetEndpoint,
                ProjectOrChannelKey = dto.ProjectOrChannelKey,
                AutoSyncActions = dto.AutoSyncActions,
                IsActive = true
            };
            _context.ConnectorConfigs.Add(existing);
        }

        await _context.SaveChangesAsync(default);

        return Ok(new ConnectorConfigDto
        {
            Id = existing.Id,
            IdeaId = existing.IdeaId,
            Type = existing.Type,
            Name = existing.Name,
            TargetEndpoint = existing.TargetEndpoint,
            ProjectOrChannelKey = existing.ProjectOrChannelKey,
            IsActive = existing.IsActive,
            AutoSyncActions = existing.AutoSyncActions,
            LastSyncTime = existing.LastSyncTime
        });
    }

    [HttpPost("webhook/{ideaId}")]
    public async Task<IActionResult> ReceiveWebhook(int ideaId, [FromBody] object payload)
    {
        await _connectorService.DispatchWebhookNotificationAsync(ideaId, "ExternalWebhookReceived", payload);
        return Ok(new { success = true, receivedAt = DateTime.UtcNow });
    }
}
