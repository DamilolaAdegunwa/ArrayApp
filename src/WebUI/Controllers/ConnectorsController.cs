#pragma warning disable
#pragma info disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectorsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IConnectorService _connectorService;
    private readonly ISender _mediator;

    public ConnectorsController(IApplicationDbContext context, IConnectorService connectorService, ISender mediator)
    {
        _context = context;
        _connectorService = connectorService;
        _mediator = mediator;
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
    public async Task<ActionResult<ConnectorConfigDto>> ConfigureConnector([FromBody] ConfigureConnectorCommand command)
    {
        var config = await _mediator.Send(command);
        return Ok(config);
    }

    [HttpPost("sync-action")]
    public async Task<ActionResult<ConnectorSyncLogDto>> SyncAction([FromBody] SyncActionToConnectorCommand command)
    {
        var log = await _mediator.Send(command);
        return Ok(log);
    }

    [HttpPost("webhook/reconcile")]
    public async Task<ActionResult<bool>> ReconcileWebhook([FromBody] ProcessInboundWebhookCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("webhook/{ideaId}")]
    public async Task<IActionResult> ReceiveWebhook(int ideaId, [FromBody] object payload)
    {
        await _connectorService.DispatchWebhookNotificationAsync(ideaId, "ExternalWebhookReceived", payload);
        return Ok(new { success = true, receivedAt = DateTimeOffset.UtcNow });
    }
}
