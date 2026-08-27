namespace ArrayApp.Application.Ideas.Commands;

public record SyncActionToConnectorCommand : IRequest<ConnectorSyncLogDto>
{
    public int ActionId { get; init; }
    public ConnectorType ConnectorType { get; init; } = ConnectorType.Jira;
    public string ActorName { get; init; } = "Integration Mesh";
}

public class SyncActionToConnectorCommandHandler : IRequestHandler<SyncActionToConnectorCommand, ConnectorSyncLogDto>
{
    private readonly IConnectorService _connectorService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SyncActionToConnectorCommandHandler> _logger;

    public SyncActionToConnectorCommandHandler(
        IConnectorService connectorService,
        IApplicationDbContext context,
        ILogger<SyncActionToConnectorCommandHandler> logger)
    {
        _connectorService = connectorService;
        _context = context;
        _logger = logger;
    }

    public async Task<ConnectorSyncLogDto> Handle(SyncActionToConnectorCommand request, CancellationToken cancellationToken)
    {
        var action = await _context.Actions
            .Include(a => a.Idea)
            .FirstOrDefaultAsync(a => a.Id == request.ActionId, cancellationToken);

        if (action == null)
        {
            throw new NotFoundException(nameof(IdeaAction), request.ActionId);
        }

        var syncLog = await _connectorService.SyncActionAsync(request.ActionId, request.ConnectorType, cancellationToken);

        // 1. Update action external metadata
        action.ExternalSystem = request.ConnectorType.ToString();
        action.ExternalReferenceKey = $"{request.ConnectorType.ToString().ToUpper()}-{action.Id}";
        action.ExternalUrl = $"https://app.{request.ConnectorType.ToString().ToLower()}.com/browse/{action.ExternalReferenceKey}";
        action.LastModificationTime = DateTimeOffset.UtcNow;

        // 2. Record Provenance Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = action.IdeaId,
            ActorName = request.ActorName,
            ActorRole = "Integration Mesh Connector",
            ActionPerformed = $"ActionSynced_{request.ConnectorType}",
            Details = $"Synced Action #{action.Id} ('{action.Title}') to {request.ConnectorType}. External Ref: {action.ExternalReferenceKey}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 3. Dispatch Domain Event
        action.Idea?.AddDomainEvent(new ActionSyncedToConnectorEvent(
            action.Id,
            action.IdeaId,
            request.ConnectorType,
            action.ExternalReferenceKey,
            action.ExternalUrl
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Action {ActionId} synced to {ConnectorType} with reference {RefKey}", action.Id, request.ConnectorType, action.ExternalReferenceKey);

        return syncLog;
    }
}
