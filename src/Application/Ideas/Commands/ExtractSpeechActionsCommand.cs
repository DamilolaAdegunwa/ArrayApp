namespace ArrayApp.Application.Ideas.Commands;

public class ExtractedSpeechOutcomesResultDto
{
    public int SessionId { get; set; }
    public int IdeaId { get; set; }
    public List<IdeaActionDto> ExtractedActions { get; set; } = new();
    public List<IdeaDecisionDto> ExtractedDecisions { get; set; } = new();
    public string RealtimeAiSummary { get; set; } = string.Empty;
    public DateTimeOffset ExtractedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record ExtractSpeechActionsCommand : IRequest<ExtractedSpeechOutcomesResultDto>
{
    public int SessionId { get; init; }
    public int IdeaId { get; init; }
    public string SpokenTranscript { get; init; } = string.Empty;
    public string SpeakerName { get; init; } = "Participant";
    public string? SpeakerUserId { get; init; }
    public ParticipantRole SpeakerRole { get; init; } = ParticipantRole.Actioner;
}

public class ExtractSpeechActionsCommandHandler : IRequestHandler<ExtractSpeechActionsCommand, ExtractedSpeechOutcomesResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ExtractSpeechActionsCommandHandler> _logger;

    public ExtractSpeechActionsCommandHandler(
        IApplicationDbContext context,
        ILogger<ExtractSpeechActionsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ExtractedSpeechOutcomesResultDto> Handle(ExtractSpeechActionsCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var result = new ExtractedSpeechOutcomesResultDto
        {
            SessionId = request.SessionId,
            IdeaId = request.IdeaId
        };

        var text = request.SpokenTranscript.ToLowerInvariant();

        // 1. Detect Spoken Decisions
        if (text.Contains("we decided") || text.Contains("agreed to") || text.Contains("approved the") || text.Contains("sign off"))
        {
            var decision = new IdeaDecision
            {
                IdeaId = idea.Id,
                SessionId = request.SessionId,
                Summary = request.SpokenTranscript,
                Rationale = $"Spoken consensus recorded by {request.SpeakerName} ({request.SpeakerRole}) during live session.",
                Context = "Live WebRTC Audio Diarization Stream",
                DecidedByUserId = request.SpeakerUserId,
                DecidedAt = DateTimeOffset.UtcNow,
                CreationTime = DateTimeOffset.UtcNow
            };
            _context.Decisions.Add(decision);
            await _context.SaveChangesAsync(cancellationToken);

            result.ExtractedDecisions.Add(new IdeaDecisionDto
            {
                Id = decision.Id,
                IdeaId = decision.IdeaId,
                SessionId = decision.SessionId,
                Summary = decision.Summary,
                Rationale = decision.Rationale,
                Context = decision.Context,
                DecidedByUserName = request.SpeakerName,
                DecidedAt = decision.DecidedAt
            });
        }

        // 2. Detect Spoken Action Commitments
        if (text.Contains("will build") || text.Contains("will deploy") || text.Contains("will implement") || text.Contains("action item") || text.Contains("i will") || text.Contains("commit to"))
        {
            var action = new IdeaAction
            {
                IdeaId = idea.Id,
                SessionId = request.SessionId,
                Title = request.SpokenTranscript,
                Description = $"Extracted from spoken statement by {request.SpeakerName}: '{request.SpokenTranscript}'",
                OwnerUserId = request.SpeakerUserId ?? "unassigned",
                Status = ActionItemStatus.Todo,
                Priority = PriorityLevel.High,
                DueDate = DateTimeOffset.UtcNow.AddDays(7),
                CreationTime = DateTimeOffset.UtcNow
            };
            _context.Actions.Add(action);
            await _context.SaveChangesAsync(cancellationToken);

            result.ExtractedActions.Add(new IdeaActionDto
            {
                Id = action.Id,
                IdeaId = action.IdeaId,
                SessionId = action.SessionId,
                Title = action.Title,
                Description = action.Description,
                OwnerUserId = action.OwnerUserId,
                Status = action.Status,
                Priority = action.Priority,
                DueDate = action.DueDate
            });
        }

        result.RealtimeAiSummary = $"Diarized speech from {request.SpeakerName}: extracted {result.ExtractedActions.Count} actions and {result.ExtractedDecisions.Count} decisions.";

        // 3. Record Provenance Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = $"Diarization Engine ({request.SpeakerName})",
            ActorRole = "Speech AI Diarizer",
            ActionPerformed = "SpeechToActionsExtracted",
            Details = result.RealtimeAiSummary,
            Timestamp = DateTimeOffset.UtcNow
        });

        // 4. Dispatch Domain Event
        idea.AddDomainEvent(new SpeechActionsExtractedEvent(
            request.SessionId,
            idea.Id,
            result.ExtractedActions.Count,
            result.ExtractedDecisions.Count,
            result.RealtimeAiSummary
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Extracted {ActionsCount} actions and {DecisionsCount} decisions from speech transcript for Idea {IdeaId}",
            result.ExtractedActions.Count, result.ExtractedDecisions.Count, idea.Id);

        return result;
    }
}
