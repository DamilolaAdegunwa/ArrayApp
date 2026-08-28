namespace ArrayApp.Application.Ideas.Commands;

public class IdeaPredictionResultDto
{
    public int IdeaId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool PredictsSuccess { get; set; }
    public int WageredKarmaPoints { get; set; }
    public double CurrentImpliedProbability { get; set; }
    public string Message { get; set; } = string.Empty;
}

public record PlaceIdeaPredictionCommand : IRequest<IdeaPredictionResultDto>
{
    public int IdeaId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = "Predictor";
    public bool PredictsSuccess { get; init; } = true;
    public int WageredKarma { get; init; } = 50;
}

public class PlaceIdeaPredictionCommandHandler : IRequestHandler<PlaceIdeaPredictionCommand, IdeaPredictionResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PlaceIdeaPredictionCommandHandler> _logger;

    public PlaceIdeaPredictionCommandHandler(
        IApplicationDbContext context,
        ILogger<PlaceIdeaPredictionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaPredictionResultDto> Handle(PlaceIdeaPredictionCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var wager = Math.Max(10, request.WageredKarma);
        var impliedProb = request.PredictsSuccess ? 0.78 : 0.22;

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.UserName,
            ActorRole = "Prediction Market Trader",
            ActionPerformed = "IdeaPredictionPlaced",
            Details = $"Wagered {wager} Karma on {(request.PredictsSuccess ? "SUCCESS" : "FAILURE")} target outcome. Implied Prob: {impliedProb:P0}",
            Timestamp = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Prediction placed by {User} on Idea {IdeaId}: {Prediction} ({Wager} Karma)", request.UserName, idea.Id, request.PredictsSuccess, wager);

        return new IdeaPredictionResultDto
        {
            IdeaId = idea.Id,
            UserId = request.UserId,
            PredictsSuccess = request.PredictsSuccess,
            WageredKarmaPoints = wager,
            CurrentImpliedProbability = impliedProb,
            Message = $"Placed {wager} Karma prediction on {(request.PredictsSuccess ? "Success" : "Pivot/Failure")}."
        };
    }
}

public class BountyAttachmentResultDto
{
    public int IdeaId { get; set; }
    public string TargetType { get; set; } = "KnowledgeGap"; // KnowledgeGap, Action
    public int TargetId { get; set; }
    public decimal BountyAmount { get; set; }
    public string SponsorName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public record AttachBountyCommand : IRequest<BountyAttachmentResultDto>
{
    public int IdeaId { get; init; }
    public string TargetType { get; init; } = "KnowledgeGap";
    public int TargetId { get; init; }
    public decimal BountyAmount { get; init; } = 500m;
    public string SponsorName { get; init; } = "Sponsor";
}

public class AttachBountyCommandHandler : IRequestHandler<AttachBountyCommand, BountyAttachmentResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AttachBountyCommandHandler> _logger;

    public AttachBountyCommandHandler(
        IApplicationDbContext context,
        ILogger<AttachBountyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BountyAttachmentResultDto> Handle(AttachBountyCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.SponsorName,
            ActorRole = "Bounty Sponsor",
            ActionPerformed = "BountyAttached",
            Details = $"Attached ${request.BountyAmount:N0} micro-grant bounty to {request.TargetType} #{request.TargetId}",
            Timestamp = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Attached ${Bounty} bounty to {TargetType} #{TargetId} on Idea {IdeaId}", request.BountyAmount, request.TargetType, request.TargetId, idea.Id);

        return new BountyAttachmentResultDto
        {
            IdeaId = idea.Id,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            BountyAmount = request.BountyAmount,
            SponsorName = request.SponsorName,
            Message = $"Attached ${request.BountyAmount:N0} bounty reward for resolving {request.TargetType} #{request.TargetId}."
        };
    }
}
