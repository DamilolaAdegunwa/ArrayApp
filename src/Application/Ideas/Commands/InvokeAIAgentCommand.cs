namespace ArrayApp.Application.Ideas.Commands;

public record InvokeAIAgentCommand : IRequest<AIAgentInsightDto>
{
    public int IdeaId { get; init; }
    public int? SessionId { get; init; }
    public AIAgentType AgentType { get; init; }
    public string? CustomPrompt { get; init; }
    public string ActorName { get; init; } = "Facilitator";
}

public class InvokeAIAgentCommandHandler : IRequestHandler<InvokeAIAgentCommand, AIAgentInsightDto>
{
    private readonly IAIAgentService _aiAgentService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<InvokeAIAgentCommandHandler> _logger;

    public InvokeAIAgentCommandHandler(
        IAIAgentService aiAgentService,
        IApplicationDbContext context,
        ILogger<InvokeAIAgentCommandHandler> logger)
    {
        _aiAgentService = aiAgentService;
        _context = context;
        _logger = logger;
    }

    public async Task<AIAgentInsightDto> Handle(InvokeAIAgentCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var insightDto = await _aiAgentService.RunAgentAnalysisAsync(
            request.IdeaId,
            request.AgentType,
            request.CustomPrompt,
            request.SessionId,
            cancellationToken
        );

        // 1. Record Provenance Audit Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = $"AI Agent: {insightDto.AgentName}",
            ActorRole = "Autonomous AI Co-Worker",
            ActionPerformed = $"AIAgentInsight_{request.AgentType}",
            Details = $"Generated insight '{insightDto.Title}'. Confidence Score: {insightDto.ConfidenceScore:P1}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 2. Dispatch Domain Event
        idea.AddDomainEvent(new AIAgentInsightGeneratedEvent(
            idea.Id,
            insightDto.Id,
            request.AgentType,
            insightDto.AgentName,
            insightDto.Title,
            insightDto.ConfidenceScore
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AI Agent {AgentType} generated insight for Idea {IdeaId}: {Title}", request.AgentType, idea.Id, insightDto.Title);

        return insightDto;
    }
}
