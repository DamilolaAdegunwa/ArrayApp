namespace ArrayApp.Application.Ideas.Commands;

public record ProcessInboundWebhookCommand : IRequest<bool>
{
    public string ExternalSystem { get; init; } = "GitHub";
    public string ExternalReferenceKey { get; init; } = string.Empty;
    public string Status { get; init; } = "Closed";
    public string? ResolutionComment { get; init; }
}

public class ProcessInboundWebhookCommandHandler : IRequestHandler<ProcessInboundWebhookCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ProcessInboundWebhookCommandHandler> _logger;

    public ProcessInboundWebhookCommandHandler(
        IApplicationDbContext context,
        ILogger<ProcessInboundWebhookCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessInboundWebhookCommand request, CancellationToken cancellationToken)
    {
        var action = await _context.Actions
            .FirstOrDefaultAsync(a => a.ExternalReferenceKey == request.ExternalReferenceKey, cancellationToken);

        if (action == null)
        {
            _logger.LogWarning("Inbound webhook received for unknown external reference: {Key}", request.ExternalReferenceKey);
            return false;
        }

        var isCompleted = request.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase) ||
                          request.Status.Equals("Done", StringComparison.OrdinalIgnoreCase) ||
                          request.Status.Equals("Merged", StringComparison.OrdinalIgnoreCase);

        if (isCompleted)
        {
            action.Status = ActionItemStatus.Done;
            action.CompletedAt = DateTimeOffset.UtcNow;
        }

        action.LastModificationTime = DateTimeOffset.UtcNow;

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = action.IdeaId,
            ActorName = $"Webhook ({request.ExternalSystem})",
            ActorRole = "External Automation",
            ActionPerformed = "InboundWebhookProcessed",
            Details = $"Action #{action.Id} reconciled to {action.Status}. External status: {request.Status}",
            Timestamp = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Reconciled Action {ActionId} from {System} webhook to status {Status}", action.Id, request.ExternalSystem, action.Status);

        return true;
    }
}
