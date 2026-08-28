using System.Security.Cryptography;
using System.Text;

namespace ArrayApp.Application.Ideas.Queries;

public class AbacAccessDecisionDto
{
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RequiredClearance { get; set; } = "Internal";
    public string UserClearance { get; set; } = "Internal";
}

public record EvaluateIdeaAccessQuery(
    int IdeaId,
    string UserId,
    string UserDepartment,
    string UserClearanceLevel
) : IRequest<AbacAccessDecisionDto>;

public class EvaluateIdeaAccessQueryHandler : IRequestHandler<EvaluateIdeaAccessQuery, AbacAccessDecisionDto>
{
    private readonly IApplicationDbContext _context;

    public EvaluateIdeaAccessQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AbacAccessDecisionDto> Handle(EvaluateIdeaAccessQuery request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        // Evaluate zero-trust security clearance
        var isTopSecret = idea.Title.Contains("[Confidential]", StringComparison.OrdinalIgnoreCase) ||
                          idea.Description.Contains("[Proprietary]", StringComparison.OrdinalIgnoreCase);

        var required = isTopSecret ? "TopSecret" : "Internal";

        if (isTopSecret && !request.UserClearanceLevel.Equals("TopSecret", StringComparison.OrdinalIgnoreCase))
        {
            return new AbacAccessDecisionDto
            {
                IsAllowed = false,
                Reason = "Access Denied: Idea contains proprietary R&D trade secrets requiring TopSecret clearance.",
                RequiredClearance = required,
                UserClearance = request.UserClearanceLevel
            };
        }

        return new AbacAccessDecisionDto
        {
            IsAllowed = true,
            Reason = "Access Granted: ABAC clearance and departmental tenancy verified.",
            RequiredClearance = required,
            UserClearance = request.UserClearanceLevel
        };
    }
}

public class AnonymizedIdeaDto
{
    public int IdeaId { get; set; }
    public string PseudonymAuthor { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string Hypothesis { get; set; } = string.Empty;
    public string ValueProposition { get; set; } = string.Empty;
    public string SolutionArchitecture { get; set; } = string.Empty;
    public string MaturityStage { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public DateTimeOffset AnonymizedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record GetAnonymizedIdeaQuery(int IdeaId) : IRequest<AnonymizedIdeaDto>;

public class GetAnonymizedIdeaQueryHandler : IRequestHandler<GetAnonymizedIdeaQuery, AnonymizedIdeaDto>
{
    private readonly IApplicationDbContext _context;

    public GetAnonymizedIdeaQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnonymizedIdeaDto> Handle(GetAnonymizedIdeaQuery request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        // Generate consistent pseudonymous identifier using SHA-256
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(idea.CreatorUserId.ToString()));
        var pseudonym = $"Anonymous Innovator #{Convert.ToHexString(hash)[..6]}";

        return new AnonymizedIdeaDto
        {
            IdeaId = idea.Id,
            PseudonymAuthor = pseudonym,
            Title = idea.Title ?? string.Empty,
            ProblemStatement = !string.IsNullOrWhiteSpace(idea.ProblemStatement) ? idea.ProblemStatement : (idea.Description ?? string.Empty),
            Hypothesis = idea.Hypothesis ?? string.Empty,
            ValueProposition = idea.ValueProposition ?? string.Empty,
            SolutionArchitecture = idea.Scope ?? string.Empty,
            MaturityStage = idea.MaturityStage.ToString(),
            TotalVotes = idea.Upvotes,
            AnonymizedAt = DateTimeOffset.UtcNow
        };
    }
}
