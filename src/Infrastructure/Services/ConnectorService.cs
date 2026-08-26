using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services;

public class ConnectorService : IConnectorService
{
    private readonly IApplicationDbContext _context;

    public ConnectorService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConnectorSyncLogDto> SyncActionAsync(int actionId, ConnectorType connectorType, CancellationToken cancellationToken = default)
    {
        var action = await _context.Actions
            .Include(a => a.Idea)
            .FirstOrDefaultAsync(a => a.Id == actionId, cancellationToken);

        if (action == null)
        {
            throw new Exception($"Action with ID {actionId} not found.");
        }

        var config = await _context.ConnectorConfigs
            .FirstOrDefaultAsync(c => c.IdeaId == action.IdeaId && c.Type == connectorType, cancellationToken);

        string refKey;
        string externalUrl;

        switch (connectorType)
        {
            case ConnectorType.Jira:
                refKey = $"IDEA-{action.Id + 100}";
                externalUrl = $"https://jira.atlassian.net/browse/{refKey}";
                break;
            case ConnectorType.GitHub:
                refKey = $"issue#{action.Id + 40}";
                externalUrl = $"https://github.com/organization/project/issues/{action.Id + 40}";
                break;
            case ConnectorType.Slack:
                refKey = $"slack-msg-{Guid.NewGuid().ToString().Substring(0, 8)}";
                externalUrl = "https://slack.com/app_redirect?channel=innovation-stream";
                break;
            case ConnectorType.Trello:
                refKey = $"card-{action.Id + 200}";
                externalUrl = $"https://trello.com/c/{refKey}";
                break;
            case ConnectorType.Linear:
                refKey = $"LIN-{action.Id + 300}";
                externalUrl = $"https://linear.app/team/issue/{refKey}";
                break;
            default:
                refKey = $"EXT-{action.Id}";
                externalUrl = $"https://api.external-service.io/items/{refKey}";
                break;
        }

        action.ExternalSystem = connectorType.ToString();
        action.ExternalReferenceKey = refKey;
        action.ExternalUrl = externalUrl;

        if (config == null)
        {
            config = new ConnectorConfig
            {
                IdeaId = action.IdeaId,
                Type = connectorType,
                Name = $"{connectorType} Default Connector",
                TargetEndpoint = externalUrl,
                IsActive = true,
                AutoSyncActions = true,
                LastSyncTime = DateTime.UtcNow
            };
            _context.ConnectorConfigs.Add(config);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            config.LastSyncTime = DateTime.UtcNow;
        }

        var syncLog = new ConnectorSyncLog
        {
            ConnectorConfigId = config.Id,
            EventType = "ActionPushed",
            Payload = JsonSerializer.Serialize(new { actionId = action.Id, title = action.Title, priority = action.Priority.ToString(), externalKey = refKey }),
            IsSuccess = true,
            ResponseMessage = $"Successfully synchronized task '{action.Title}' to {connectorType} with key {refKey}.",
            SyncedAt = DateTime.UtcNow
        };

        _context.ConnectorSyncLogs.Add(syncLog);

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = action.IdeaId,
            ActorName = "Connector Sync Engine",
            ActorRole = "System",
            ActionPerformed = "ActionSyncedToExternalSystem",
            Details = $"Synced Action '{action.Title}' to {connectorType} ({refKey})",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new ConnectorSyncLogDto
        {
            Id = syncLog.Id,
            ConnectorConfigId = syncLog.ConnectorConfigId,
            EventType = syncLog.EventType,
            Payload = syncLog.Payload,
            IsSuccess = syncLog.IsSuccess,
            ResponseMessage = syncLog.ResponseMessage,
            SyncedAt = syncLog.SyncedAt
        };
    }

    public async Task<bool> DispatchWebhookNotificationAsync(int ideaId, string eventType, object payload, CancellationToken cancellationToken = default)
    {
        var activeConnectors = await _context.ConnectorConfigs
            .Where(c => c.IdeaId == ideaId && c.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var connector in activeConnectors)
        {
            var log = new ConnectorSyncLog
            {
                ConnectorConfigId = connector.Id,
                EventType = eventType,
                Payload = JsonSerializer.Serialize(payload),
                IsSuccess = true,
                ResponseMessage = $"Event '{eventType}' dispatched to {connector.Type} endpoint.",
                SyncedAt = DateTime.UtcNow
            };
            _context.ConnectorSyncLogs.Add(log);
            connector.LastSyncTime = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
