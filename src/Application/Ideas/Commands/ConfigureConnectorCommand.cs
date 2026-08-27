namespace ArrayApp.Application.Ideas.Commands;

public record ConfigureConnectorCommand : IRequest<ConnectorConfigDto>
{
    public int IdeaId { get; init; }
    public ConnectorType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TargetEndpoint { get; init; } = string.Empty;
    public string? ApiKey { get; init; }
    public string? ProjectOrChannelKey { get; init; }
    public bool AutoSyncActions { get; init; } = true;
    public string ActorName { get; init; } = "DevOps Lead";
}

public class ConfigureConnectorCommandHandler : IRequestHandler<ConfigureConnectorCommand, ConnectorConfigDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ConfigureConnectorCommandHandler> _logger;

    public ConfigureConnectorCommandHandler(
        IApplicationDbContext context,
        ILogger<ConfigureConnectorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ConnectorConfigDto> Handle(ConfigureConnectorCommand request, CancellationToken cancellationToken)
    {
        var config = await _context.ConnectorConfigs
            .FirstOrDefaultAsync(c => c.IdeaId == request.IdeaId && c.Type == request.Type, cancellationToken);

        if (config == null)
        {
            config = new ConnectorConfig
            {
                IdeaId = request.IdeaId,
                Type = request.Type,
                Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : $"{request.Type} Connector",
                TargetEndpoint = request.TargetEndpoint,
                ApiKeyEncrypted = request.ApiKey,
                ProjectOrChannelKey = request.ProjectOrChannelKey,
                AutoSyncActions = request.AutoSyncActions,
                IsActive = true,
                LastSyncTime = DateTimeOffset.UtcNow,
                CreationTime = DateTimeOffset.UtcNow
            };
            _context.ConnectorConfigs.Add(config);
        }
        else
        {
            config.Name = request.Name;
            config.TargetEndpoint = request.TargetEndpoint;
            if (!string.IsNullOrWhiteSpace(request.ApiKey)) config.ApiKeyEncrypted = request.ApiKey;
            config.ProjectOrChannelKey = request.ProjectOrChannelKey;
            config.AutoSyncActions = request.AutoSyncActions;
            config.LastSyncTime = DateTimeOffset.UtcNow;
            config.LastModificationTime = DateTimeOffset.UtcNow;
        }

        // Provenance Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = request.IdeaId,
            ActorName = request.ActorName,
            ActorRole = "Integration Admin",
            ActionPerformed = $"ConnectorConfigured_{request.Type}",
            Details = $"Configured {request.Type} connector target: {request.TargetEndpoint}",
            Timestamp = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Connector {Type} configured for Idea {IdeaId}", request.Type, request.IdeaId);

        return new ConnectorConfigDto
        {
            Id = config.Id,
            IdeaId = config.IdeaId,
            Type = config.Type,
            Name = config.Name,
            TargetEndpoint = config.TargetEndpoint,
            ProjectOrChannelKey = config.ProjectOrChannelKey,
            IsActive = config.IsActive,
            AutoSyncActions = config.AutoSyncActions,
            LastSyncTime = config.LastSyncTime
        };
    }
}
