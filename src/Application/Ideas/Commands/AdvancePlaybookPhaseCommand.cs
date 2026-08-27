namespace ArrayApp.Application.Ideas.Commands;

public class PlaybookPhaseProgressDto
{
    public int SessionId { get; set; }
    public string PlaybookId { get; set; } = string.Empty;
    public int CurrentPhaseNumber { get; set; }
    public string CurrentPhaseTitle { get; set; } = string.Empty;
    public int PhaseDurationMinutes { get; set; }
    public string PhaseGoal { get; set; } = string.Empty;
    public string FacilitatorInstructions { get; set; } = string.Empty;
    public List<string> SuggestedPrompts { get; set; } = new();
    public DateTimeOffset PhaseStartedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsFinalPhase { get; set; }
}

public record AdvancePlaybookPhaseCommand : IRequest<PlaybookPhaseProgressDto>
{
    public int SessionId { get; init; }
    public string PlaybookId { get; init; } = "brainstorm";
    public int TargetPhaseNumber { get; init; } = 1;
    public string FacilitatorName { get; init; } = "Facilitator";
}

public class AdvancePlaybookPhaseCommandHandler : IRequestHandler<AdvancePlaybookPhaseCommand, PlaybookPhaseProgressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISessionPlaybookService _playbookService;
    private readonly ILogger<AdvancePlaybookPhaseCommandHandler> _logger;

    public AdvancePlaybookPhaseCommandHandler(
        IApplicationDbContext context,
        ISessionPlaybookService playbookService,
        ILogger<AdvancePlaybookPhaseCommandHandler> logger)
    {
        _context = context;
        _playbookService = playbookService;
        _logger = logger;
    }

    public async Task<PlaybookPhaseProgressDto> Handle(AdvancePlaybookPhaseCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException(nameof(Session), request.SessionId);
        }

        var playbook = await _playbookService.GetPlaybookTemplateAsync(request.PlaybookId, cancellationToken);
        var targetPhase = playbook.Phases.FirstOrDefault(p => p.PhaseNumber == request.TargetPhaseNumber)
                          ?? playbook.Phases.First();

        var isFinalPhase = targetPhase.PhaseNumber >= playbook.Phases.Count;

        // 1. Update session status if starting
        if (targetPhase.PhaseNumber == 1 && session.ActualStartTime == null)
        {
            session.ActualStartTime = DateTimeOffset.UtcNow;
            session.SessionStatus = SessionStatus.Live;
        }

        // 2. Record Provenance Audit Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = session.PrimaryIdeaId ?? 0,
            ActorName = request.FacilitatorName,
            ActorRole = "Facilitator",
            ActionPerformed = "PlaybookPhaseAdvanced",
            Details = $"Session #{session.Id} ('{session.Name}') advanced to Phase {targetPhase.PhaseNumber}: '{targetPhase.Title}' in playbook '{playbook.Name}'. Goal: {targetPhase.Goal}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 3. Dispatch Domain Event
        session.AddDomainEvent(new PlaybookPhaseAdvancedEvent(
            session.Id,
            request.PlaybookId,
            targetPhase.PhaseNumber - 1,
            targetPhase.PhaseNumber,
            targetPhase.Title,
            targetPhase.DurationMinutes
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Session {SessionId} advanced to Playbook Phase {Phase}: {Title}", session.Id, targetPhase.PhaseNumber, targetPhase.Title);

        return new PlaybookPhaseProgressDto
        {
            SessionId = session.Id,
            PlaybookId = request.PlaybookId,
            CurrentPhaseNumber = targetPhase.PhaseNumber,
            CurrentPhaseTitle = targetPhase.Title,
            PhaseDurationMinutes = targetPhase.DurationMinutes,
            PhaseGoal = targetPhase.Goal,
            FacilitatorInstructions = targetPhase.FacilitatorInstructions,
            SuggestedPrompts = targetPhase.SuggestedPrompts,
            PhaseStartedAt = DateTimeOffset.UtcNow,
            IsFinalPhase = isFinalPhase
        };
    }
}
