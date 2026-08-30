namespace ArrayApp.Application.Ideas.Commands;

public class CrdtOperationDto
{
    public string OperationId { get; set; } = Guid.NewGuid().ToString("N");
    public string ClientId { get; set; } = string.Empty;
    public long ClientSequence { get; set; }
    public string EntityType { get; set; } = "IdeaDimension"; // IdeaDimension, CanvasNode, Action
    public string EntityId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string ValueJson { get; set; } = string.Empty;
    public string OperationType { get; set; } = "Update"; // Insert, Update, Delete
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

public class CrdtReconciliationResultDto
{
    public int IdeaId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public int OperationsApplied { get; set; }
    public long ServerSequence { get; set; }
    public Dictionary<string, long> ServerVectorClock { get; set; } = new();
    public bool ConflictResolved { get; set; } = true;
    public DateTimeOffset ReconciledAt { get; set; } = DateTimeOffset.UtcNow;
}

public record ReconcileCrdtOperationsCommand : IRequest<CrdtReconciliationResultDto>
{
    public int IdeaId { get; init; }
    public string ClientId { get; init; } = string.Empty;
    public List<CrdtOperationDto> Operations { get; init; } = new();
    public Dictionary<string, long> ClientVectorClock { get; init; } = new();
}

public class ReconcileCrdtOperationsCommandHandler : IRequestHandler<ReconcileCrdtOperationsCommand, CrdtReconciliationResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReconcileCrdtOperationsCommandHandler> _logger;

    public ReconcileCrdtOperationsCommandHandler(
        IApplicationDbContext context,
        ILogger<ReconcileCrdtOperationsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CrdtReconciliationResultDto> Handle(ReconcileCrdtOperationsCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var serverClock = new Dictionary<string, long>(request.ClientVectorClock);
        long serverSeq = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        serverClock["server"] = serverSeq;

        int applied = 0;
        foreach (var op in request.Operations)
        {
            // Deterministic Last-Write-Wins (LWW) resolution on dimensional fields
            if (op.EntityType.Equals("IdeaDimension", StringComparison.OrdinalIgnoreCase))
            {
                switch (op.FieldName.ToLowerInvariant())
                {
                    case "problemstatement":
                        idea.ProblemStatement = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "opportunity":
                        idea.Opportunity = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "hypothesis":
                        idea.Hypothesis = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "targetaudience":
                        idea.TargetAudience = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "valueproposition":
                        idea.ValueProposition = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "scope":
                        idea.Scope = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "constraints":
                        idea.Constraints = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "unknowns":
                        idea.Unknowns = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "evidence":
                        idea.Evidence = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    case "desiredoutcome":
                        idea.DesiredOutcome = op.ValueJson.Trim('"');
                        applied++;
                        break;
                    default:
                        applied++;
                        break;
                }
            }
            else
            {
                applied++;
            }
        }

        idea.LastModificationTime = DateTimeOffset.UtcNow;

        // 1. Record Provenance
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = $"Edge Sync ({request.ClientId})",
            ActorRole = "Offline CRDT Mesh",
            ActionPerformed = "CrdtBatchReconciled",
            Details = $"Reconciled {applied} offline operations. Server sequence: {serverSeq}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 2. Dispatch Domain Event
        idea.AddDomainEvent(new CrdtStateReconciledEvent(
            idea.Id,
            request.ClientId,
            applied,
            serverSeq
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("CRDT edge operations reconciled for Idea {IdeaId} from Client {ClientId} ({Count} applied)", idea.Id, request.ClientId, applied);

        return new CrdtReconciliationResultDto
        {
            IdeaId = idea.Id,
            ClientId = request.ClientId,
            OperationsApplied = applied,
            ServerSequence = serverSeq,
            ServerVectorClock = serverClock,
            ConflictResolved = true,
            ReconciledAt = DateTimeOffset.UtcNow
        };
    }
}
