namespace ArrayApp.Application.Ideas.Commands;

public class BlindReviewResultDto
{
    public int IdeaId { get; set; }
    public string ReviewerPseudonym { get; set; } = string.Empty;
    public double Score { get; set; }
    public string QualitativeCritique { get; set; } = string.Empty;
    public string Status { get; set; } = "Recorded";
    public DateTimeOffset ReviewedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record AnonymizedReviewCompletedCommand : IRequest<BlindReviewResultDto>
{
    public int IdeaId { get; init; }
    public string ReviewerPseudonym { get; init; } = "Evaluator-Alpha";
    public double Score { get; init; } = 8.5;
    public string QualitativeCritique { get; init; } = string.Empty;
    public string Recommendation { get; init; } = "Fund Prototype";
}

public class AnonymizedReviewCompletedCommandHandler : IRequestHandler<AnonymizedReviewCompletedCommand, BlindReviewResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AnonymizedReviewCompletedCommandHandler> _logger;

    public AnonymizedReviewCompletedCommandHandler(
        IApplicationDbContext context,
        ILogger<AnonymizedReviewCompletedCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BlindReviewResultDto> Handle(AnonymizedReviewCompletedCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        // 1. Record Provenance with Zero Submitter/Reviewer Attribution
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.ReviewerPseudonym,
            ActorRole = "Blind Evaluation Committee",
            ActionPerformed = "BlindReviewCompleted",
            Details = $"Score: {request.Score}/10. Recommendation: {request.Recommendation}. Critique: {request.QualitativeCritique}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 2. Dispatch Domain Event
        idea.AddDomainEvent(new BlindReviewSubmittedEvent(
            idea.Id,
            request.ReviewerPseudonym,
            request.Score,
            request.QualitativeCritique
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Blind review recorded for Idea {IdeaId} by {Pseudonym} with score {Score}", idea.Id, request.ReviewerPseudonym, request.Score);

        return new BlindReviewResultDto
        {
            IdeaId = idea.Id,
            ReviewerPseudonym = request.ReviewerPseudonym,
            Score = request.Score,
            QualitativeCritique = request.QualitativeCritique,
            Status = "Recorded",
            ReviewedAt = DateTimeOffset.UtcNow
        };
    }
}
