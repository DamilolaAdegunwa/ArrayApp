using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class ConnectorConfig : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public ConnectorType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TargetEndpoint { get; set; } = string.Empty;
    public string? ApiKeyEncrypted { get; set; }
    public string? ProjectOrChannelKey { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AutoSyncActions { get; set; } = true;
    public DateTime? LastSyncTime { get; set; }
    public List<ConnectorSyncLog> SyncLogs { get; set; } = new();
}

public class ConnectorSyncLog : BaseAuditableEntity, IAggregateRoot
{
    public int ConnectorConfigId { get; set; }
    public ConnectorConfig? ConnectorConfig { get; set; }

    public string EventType { get; set; } = string.Empty; // "ActionPushed", "WebhookReceived", "StatusUpdated"
    public string Payload { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = true;
    public string? ResponseMessage { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}
