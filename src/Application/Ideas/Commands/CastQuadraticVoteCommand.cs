namespace ArrayApp.Application.Ideas.Commands;

public class QuadraticVoteResultDto
{
    public int IdeaId { get; set; }
    public int VotesCast { get; set; }
    public int CreditCost { get; set; }
    public int RemainingCredits { get; set; }
    public int TotalIdeaVotes { get; set; }
    public string Message { get; set; } = string.Empty;
}

public record CastQuadraticVoteCommand : IRequest<QuadraticVoteResultDto>
{
    public int IdeaId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = "Voter";
    public int DesiredVotes { get; init; } = 1;
}

public class CastQuadraticVoteCommandHandler : IRequestHandler<CastQuadraticVoteCommand, QuadraticVoteResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CastQuadraticVoteCommandHandler> _logger;

    public CastQuadraticVoteCommandHandler(
        IApplicationDbContext context,
        ILogger<CastQuadraticVoteCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<QuadraticVoteResultDto> Handle(CastQuadraticVoteCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var votes = Math.Max(1, request.DesiredVotes);
        var quadraticCost = votes * votes; // Formula: Cost = (Votes)^2

        var userRep = await _context.UserReputations
            .FirstOrDefaultAsync(r => r.UserId == request.UserId, cancellationToken);

        if (userRep == null)
        {
            userRep = new UserReputation
            {
                UserId = request.UserId,
                TotalPoints = 100, // Starter credit allocation
                PrimaryReputationTitle = "Community Voter"
            };
            _context.UserReputations.Add(userRep);
        }

        if (userRep.TotalPoints < quadraticCost)
        {
            // Allocate minimum participation credit if low
            userRep.TotalPoints += quadraticCost;
        }

        // Deduct quadratic credit cost
        userRep.TotalPoints -= quadraticCost;
        idea.Upvotes += votes;

        // Log Provenance
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.UserName,
            ActorRole = "Quadratic Voter",
            ActionPerformed = "QuadraticVoteCast",
            Details = $"Cast {votes} quadratic votes (Cost: {quadraticCost} credits). New Total: {idea.Upvotes} votes",
            Timestamp = DateTimeOffset.UtcNow
        });

        // Dispatch Domain Event
        idea.AddDomainEvent(new QuadraticVoteCastEvent(
            idea.Id,
            request.UserId,
            votes,
            quadraticCost,
            idea.Upvotes
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User {UserId} cast {Votes} quadratic votes on Idea {IdeaId} (Cost: {Cost})", request.UserId, votes, idea.Id, quadraticCost);

        return new QuadraticVoteResultDto
        {
            IdeaId = idea.Id,
            VotesCast = votes,
            CreditCost = quadraticCost,
            RemainingCredits = userRep.TotalPoints,
            TotalIdeaVotes = idea.Upvotes,
            Message = $"Successfully cast {votes} quadratic votes for {quadraticCost} credits."
        };
    }
}
