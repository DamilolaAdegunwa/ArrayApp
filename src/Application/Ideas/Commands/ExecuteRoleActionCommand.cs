namespace ArrayApp.Application.Ideas.Commands;

public record ExecuteRoleActionCommand : IRequest<RoleActionResultDto>
{
    public int IdeaId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string ActorName { get; init; } = string.Empty;
    public ParticipantRole Role { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public decimal? Amount { get; init; }
    public int? TargetEntityId { get; init; }
}

public class ExecuteRoleActionCommandHandler : IRequestHandler<ExecuteRoleActionCommand, RoleActionResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ExecuteRoleActionCommandHandler> _logger;

    public ExecuteRoleActionCommandHandler(
        IApplicationDbContext context,
        ILogger<ExecuteRoleActionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RoleActionResultDto> Handle(ExecuteRoleActionCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .Include(i => i.Actions)
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        int pointsAwarded = 0;
        string resultMessage = string.Empty;
        string newBadge = string.Empty;
        BadgeType badgeType = BadgeType.KnowledgeContributor;

        switch (request.Role)
        {
            case ParticipantRole.Student:
                pointsAwarded = 25;
                resultMessage = $"Student Question logged: '{request.Payload}'. Socratic AI analysis queued.";
                newBadge = "Curious Mind";
                badgeType = BadgeType.ProblemSolver;
                break;

            case ParticipantRole.Sponsor:
                pointsAwarded = 150;
                var pledge = request.Amount ?? 10000m;
                resultMessage = $"Sponsorship pledge of ${pledge:N0} committed by {request.ActorName}.";
                newBadge = "Patron of Innovation";
                badgeType = BadgeType.IdeaSponsor;
                break;

            case ParticipantRole.Professional:
                pointsAwarded = 100;
                if (request.TargetEntityId.HasValue)
                {
                    var gap = idea.KnowledgeGaps.FirstOrDefault(g => g.Id == request.TargetEntityId.Value);
                    if (gap != null)
                    {
                        gap.Status = KnowledgeGapStatus.Resolved;
                        gap.ResolutionDetails = request.Payload;
                        gap.ResolvedAt = DateTimeOffset.UtcNow;
                        gap.AssignedToUserId = request.UserId;
                    }
                }
                resultMessage = $"Domain knowledge gap successfully resolved with expert verification.";
                newBadge = "Domain Oracle";
                badgeType = BadgeType.ExpertMentor;
                break;

            case ParticipantRole.Authority:
                pointsAwarded = 120;
                resultMessage = $"Regulatory & Institutional Sign-Off granted by {request.ActorName}.";
                newBadge = "Guardian of Safety";
                badgeType = BadgeType.ActionLeader;
                break;

            case ParticipantRole.Actioner:
                pointsAwarded = 80;
                var actionItem = new IdeaAction
                {
                    IdeaId = idea.Id,
                    Title = !string.IsNullOrWhiteSpace(request.Payload) ? request.Payload : "Committed Milestone Implementation",
                    OwnerUserId = request.UserId,
                    Status = ActionItemStatus.InProgress,
                    Priority = PriorityLevel.High,
                    DueDate = DateTimeOffset.UtcNow.AddDays(7),
                    CreationTime = DateTimeOffset.UtcNow
                };
                _context.Actions.Add(actionItem);
                resultMessage = $"Action Item claimed by {request.ActorName} with 7-day target ETA.";
                newBadge = "Velocity Driver";
                badgeType = BadgeType.IdeaBuilder;
                break;

            case ParticipantRole.Audience:
                pointsAwarded = 10;
                idea.Upvotes++;
                resultMessage = $"Reaction '{request.Payload}' broadcasted to live audience mesh.";
                newBadge = "Active Voice";
                badgeType = BadgeType.KnowledgeContributor;
                break;

            case ParticipantRole.Researcher:
                pointsAwarded = 90;
                if (!string.IsNullOrWhiteSpace(request.Payload))
                {
                    idea.Evidence = string.IsNullOrWhiteSpace(idea.Evidence)
                        ? request.Payload
                        : $"{idea.Evidence}\n[Research by {request.ActorName}]: {request.Payload}";
                }
                resultMessage = $"Empirical research and citation evidence attached to proposal.";
                newBadge = "Evidence Master";
                badgeType = BadgeType.KnowledgeContributor;
                break;

            case ParticipantRole.Creator:
                pointsAwarded = 70;
                resultMessage = $"Visual canvas artifact updated by {request.ActorName}.";
                newBadge = "Architect";
                badgeType = BadgeType.IdeaCatalyst;
                break;

            case ParticipantRole.Experimenter:
                pointsAwarded = 110;
                if (request.TargetEntityId.HasValue)
                {
                    var exp = idea.Experiments.FirstOrDefault(e => e.Id == request.TargetEntityId.Value);
                    if (exp != null)
                    {
                        exp.ActualResult = request.Payload;
                        exp.Status = ExperimentStatus.Validated;
                        exp.CompletedAt = DateTimeOffset.UtcNow;
                    }
                }
                resultMessage = $"Hypothesis experiment metric logged and marked Validated.";
                newBadge = "Empirical Scientist";
                badgeType = BadgeType.Experimenter;
                break;

            case ParticipantRole.Connector:
                pointsAwarded = 85;
                resultMessage = $"Strategic partner lead '{request.Payload}' connected to idea workstream.";
                newBadge = "Ecosystem Bridge";
                badgeType = BadgeType.Connector;
                break;

            default:
                pointsAwarded = 20;
                resultMessage = $"Contribution recorded for role {request.Role}.";
                newBadge = "Contributor";
                badgeType = BadgeType.KnowledgeContributor;
                break;
        }

        // 1. Record Provenance Log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.ActorName,
            ActorRole = request.Role.ToString(),
            ActionPerformed = $"RoleAction_{request.ActionType}",
            Details = $"{resultMessage} (Payload: {request.Payload})",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 2. Award User Reputation & Badge
        var userRep = await _context.UserReputations
            .Include(r => r.Badges)
            .FirstOrDefaultAsync(r => r.UserId == request.UserId, cancellationToken);

        if (userRep == null)
        {
            userRep = new UserReputation
            {
                UserId = request.UserId,
                TotalPoints = pointsAwarded,
                PrimaryReputationTitle = newBadge
            };
            _context.UserReputations.Add(userRep);
        }
        else
        {
            userRep.TotalPoints += pointsAwarded;
            if (!string.IsNullOrWhiteSpace(newBadge))
            {
                userRep.PrimaryReputationTitle = newBadge;
            }
        }

        userRep.Badges.Add(new UserBadge
        {
            BadgeType = badgeType,
            Title = newBadge,
            Description = $"Awarded for executing {request.Role} action '{request.ActionType}' on Idea #{idea.Id}",
            Icon = "fa-medal",
            AwardedAt = DateTimeOffset.UtcNow
        });

        // 3. Dispatch Domain Event
        idea.AddDomainEvent(new RoleActionExecutedEvent(idea.Id, request.ActorName, request.Role, request.ActionType, resultMessage, pointsAwarded));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Role action {ActionType} by {Actor} ({Role}) executed on Idea {IdeaId}. Points: +{Points}", request.ActionType, request.ActorName, request.Role, idea.Id, pointsAwarded);

        return new RoleActionResultDto
        {
            Success = true,
            Message = resultMessage,
            ReputationPointsAwarded = pointsAwarded,
            NewBadgeEarned = newBadge,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}
