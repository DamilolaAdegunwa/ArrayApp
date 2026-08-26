using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Entities.TagAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services;

public class IdeaProductService : IIdeaProductService
{
    private readonly IApplicationDbContext _context;
    private readonly IReputationService _reputationService;

    public IdeaProductService(IApplicationDbContext context, IReputationService reputationService)
    {
        _context = context;
        _reputationService = reputationService;
    }

    public async Task<List<IdeaProductDto>> GetIdeasAsync(IdeaMaturityStage? stage = null, int? categoryId = null, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.Author)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Hypotheses)
            .Include(i => i.Experiments)
            .Include(i => i.Decisions)
            .Include(i => i.Actions)
            .Include(i => i.Outcomes)
            .Include(i => i.Subscriptions)
            .AsNoTracking();

        if (stage.HasValue)
        {
            query = query.Where(i => i.MaturityStage == stage.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(i => (i.Title != null && i.Title.ToLower().Contains(term)) ||
                                     (i.Description != null && i.Description.ToLower().Contains(term)) ||
                                     (i.ProblemStatement != null && i.ProblemStatement.ToLower().Contains(term)));
        }

        var ideas = await query.OrderByDescending(i => i.CreationTime).ToListAsync(cancellationToken);

        return ideas.Select(MapToDto).ToList();
    }

    public async Task<IdeaProductDto?> GetIdeaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.Author)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Hypotheses)
            .Include(i => i.Experiments)
            .Include(i => i.Decisions)
            .Include(i => i.Actions)
            .Include(i => i.Outcomes)
            .Include(i => i.Subscriptions)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (idea == null) return null;

        idea.ViewsCount++;
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(idea);
    }

    public async Task<IdeaProductDto> CreateIdeaAsync(CreateIdeaProductDto dto, string? userId, CancellationToken cancellationToken = default)
    {
        var category = dto.CategoryId.HasValue
            ? await _context.Categories.FindAsync(new object[] { dto.CategoryId.Value }, cancellationToken)
            : await _context.Categories.FirstOrDefaultAsync(cancellationToken);

        if (category == null)
        {
            category = new Category { Name = "General Innovation", Description = "General Ideas & Concepts" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var idea = new Idea
        {
            Title = dto.Title,
            Tagline = dto.Tagline,
            Description = dto.Description,
            ProblemStatement = dto.ProblemStatement,
            Opportunity = dto.Opportunity,
            Hypothesis = dto.Hypothesis,
            TargetAudience = dto.TargetAudience,
            ValueProposition = dto.ValueProposition,
            Constraints = dto.Constraints,
            Unknowns = dto.Unknowns,
            Evidence = dto.Evidence,
            DesiredOutcome = dto.DesiredOutcome,
            MaturityStage = IdeaMaturityStage.Raw,
            Visibility = dto.Visibility,
            CategoryId = category.Id,
            CreatorUserId = int.TryParse(userId, out var uid) ? uid : 0,
            Rating = 5.0,
            Upvotes = 1,
            FollowersCount = 1,
            ViewsCount = 1
        };

        // Create default persistent discussion channels
        idea.DiscussionChannels.Add(new DiscussionChannel { Name = "general", Description = "General discussion and open debates", IsDefault = true });
        idea.DiscussionChannels.Add(new DiscussionChannel { Name = "research", Description = "Evidence, papers, benchmarks, and prior art" });
        idea.DiscussionChannels.Add(new DiscussionChannel { Name = "critique", Description = "Assumptions challenge, SWOT, and risk analysis" });
        idea.DiscussionChannels.Add(new DiscussionChannel { Name = "implementation", Description = "Engineering, tasks, prototypes, and milestones" });

        // Add initial knowledge gaps if provided
        if (dto.InitialKnowledgeGaps != null)
        {
            foreach (var gapText in dto.InitialKnowledgeGaps.Where(g => !string.IsNullOrWhiteSpace(g)))
            {
                idea.KnowledgeGaps.Add(new KnowledgeGap
                {
                    Title = gapText,
                    Description = $"Initial knowledge gap identified during concept capture: {gapText}",
                    DomainArea = "General",
                    Priority = PriorityLevel.High,
                    Status = KnowledgeGapStatus.Open
                });
            }
        }

        // Add author as Creator subscription
        if (!string.IsNullOrEmpty(userId))
        {
            idea.Subscriptions.Add(new IdeaSubscription
            {
                UserId = userId,
                Role = ParticipantRole.Creator,
                RoleJustification = "Originator / Idea Creator",
                ContributionsCount = 1
            });
        }

        _context.Ideas.Add(idea);

        // Add initial provenance log
        idea.ProvenanceLogs.Add(new ProvenanceLog
        {
            ActorName = !string.IsNullOrEmpty(userId) ? userId : "Creator",
            ActorRole = "Creator",
            ActionPerformed = "IdeaCreated",
            Details = $"Idea '{idea.Title}' created in stage Raw.",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(userId))
        {
            await _reputationService.AwardPointsAsync(userId, 25, "Created a new Idea Product", cancellationToken);
        }

        return MapToDto(idea);
    }

    public async Task<bool> UpdateMaturityStageAsync(int ideaId, IdeaMaturityStage newStage, string? rationale, string? userId, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);
        if (idea == null) return false;

        var previousStage = idea.MaturityStage;
        idea.MaturityStage = newStage;

        idea.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = !string.IsNullOrEmpty(userId) ? userId : "Contributor",
            ActorRole = "Actioner",
            ActionPerformed = "MaturityAdvanced",
            Details = $"Stage advanced from {previousStage} to {newStage}. Rationale: {rationale ?? "Criteria satisfied."}",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(userId))
        {
            await _reputationService.AwardPointsAsync(userId, 30, $"Advanced idea stage to {newStage}", cancellationToken);
        }

        return true;
    }

    public async Task<IdeaProductDto> ForkIdeaAsync(int ideaId, ForkIdeaDto dto, string? userId, CancellationToken cancellationToken = default)
    {
        var sourceIdea = await _context.Ideas
            .Include(i => i.KnowledgeGaps)
            .FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);

        if (sourceIdea == null)
        {
            throw new Exception($"Source idea with ID {ideaId} not found.");
        }

        var forkedIdea = new Idea
        {
            Title = string.IsNullOrWhiteSpace(dto.NewTitle) ? $"{sourceIdea.Title} (Fork)" : dto.NewTitle,
            Tagline = sourceIdea.Tagline,
            Description = sourceIdea.Description,
            ProblemStatement = sourceIdea.ProblemStatement,
            Opportunity = sourceIdea.Opportunity,
            Hypothesis = sourceIdea.Hypothesis,
            TargetAudience = sourceIdea.TargetAudience,
            ValueProposition = sourceIdea.ValueProposition,
            Constraints = sourceIdea.Constraints,
            Unknowns = sourceIdea.Unknowns,
            Evidence = sourceIdea.Evidence,
            DesiredOutcome = sourceIdea.DesiredOutcome,
            MaturityStage = IdeaMaturityStage.Exploring,
            ForkedFromIdeaId = sourceIdea.Id,
            CategoryId = sourceIdea.CategoryId,
            CreatorUserId = int.TryParse(userId, out var uid2) ? uid2 : 0,
            Rating = 5.0,
            Upvotes = 1,
            FollowersCount = 1,
            ViewsCount = 1
        };

        forkedIdea.DiscussionChannels.Add(new DiscussionChannel { Name = "general", Description = "General discussion on this fork", IsDefault = true });
        forkedIdea.DiscussionChannels.Add(new DiscussionChannel { Name = "research", Description = "Research and lineage evidence" });

        _context.Ideas.Add(forkedIdea);

        forkedIdea.ProvenanceLogs.Add(new ProvenanceLog
        {
            ActorName = !string.IsNullOrEmpty(userId) ? userId : "Innovator",
            ActorRole = "Creator",
            ActionPerformed = "IdeaForked",
            Details = $"Forked from Idea #{sourceIdea.Id} ('{sourceIdea.Title}'). Reason: {dto.ForkReason}",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(userId))
        {
            await _reputationService.AwardPointsAsync(userId, 20, "Forked an idea to explore a new angle", cancellationToken);
        }

        return MapToDto(forkedIdea);
    }

    public async Task<IdeaGraphDto> GetIdeaGraphAsync(int? focusIdeaId = null, CancellationToken cancellationToken = default)
    {
        var ideas = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.Actions)
            .Include(i => i.Outcomes)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var graph = new IdeaGraphDto();

        foreach (var idea in ideas)
        {
            graph.Nodes.Add(new IdeaGraphNodeDto
            {
                Id = idea.Id,
                Label = idea.Title ?? $"Idea #{idea.Id}",
                Stage = idea.MaturityStage.ToString(),
                Category = idea.Category?.Name ?? "General",
                ActionsCount = idea.Actions.Count,
                OutcomesCount = idea.Outcomes.Count
            });

            if (idea.ForkedFromIdeaId.HasValue)
            {
                graph.Links.Add(new IdeaGraphLinkDto
                {
                    Source = idea.ForkedFromIdeaId.Value,
                    Target = idea.Id,
                    Type = "Fork"
                });
            }

            if (idea.ParentIdeaId.HasValue)
            {
                graph.Links.Add(new IdeaGraphLinkDto
                {
                    Source = idea.ParentIdeaId.Value,
                    Target = idea.Id,
                    Type = "ParentChild"
                });
            }

            if (idea.MergedIntoIdeaId.HasValue)
            {
                graph.Links.Add(new IdeaGraphLinkDto
                {
                    Source = idea.Id,
                    Target = idea.MergedIntoIdeaId.Value,
                    Type = "Merged"
                });
            }
        }

        return graph;
    }

    public async Task<InnovationPipelineAnalyticsDto> GetPipelineAnalyticsAsync(CancellationToken cancellationToken = default)
    {
        var ideas = await _context.Ideas
            .Include(i => i.Actions)
            .Include(i => i.Outcomes)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var outcomes = await _context.Outcomes.AsNoTracking().ToListAsync(cancellationToken);
        var sessions = await _context.Sessions.AsNoTracking().ToListAsync(cancellationToken);

        var analytics = new InnovationPipelineAnalyticsDto
        {
            TotalIdeas = ideas.Count,
            RawCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Raw),
            ExploringCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Exploring),
            StructuredCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Structured),
            ValidatingCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Validating),
            ExperimentingCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Experimenting),
            PlannedCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Planned),
            BuildingCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Building),
            ImplementedCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Implemented),
            MeasuredCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Measured),
            EvolvingCount = ideas.Count(i => i.MaturityStage == IdeaMaturityStage.Evolving),

            TotalEstimatedCostSavings = outcomes.Sum(o => o.EstimatedCostSavings),
            TotalRevenueGenerated = outcomes.Sum(o => o.RevenueGenerated),
            TotalImpactedUsers = outcomes.Sum(o => o.ImpactedUsersCount),
            TotalActionsCompleted = ideas.SelectMany(i => i.Actions).Count(a => a.Status == ActionItemStatus.Done),
            TotalSessionsHosted = sessions.Count,
            AverageTimeToFirstActionDays = 3.8
        };

        analytics.IdeaToOutcomeConversionRate = ideas.Count > 0
            ? Math.Round((double)ideas.Count(i => i.Outcomes.Count > 0) / ideas.Count * 100, 1)
            : 0;

        return analytics;
    }

    private static IdeaProductDto MapToDto(Idea idea)
    {
        return new IdeaProductDto
        {
            Id = idea.Id,
            Title = idea.Title ?? string.Empty,
            Tagline = idea.Tagline ?? string.Empty,
            Description = idea.Description ?? string.Empty,
            Content = idea.Content ?? string.Empty,
            ProblemStatement = idea.ProblemStatement ?? string.Empty,
            Opportunity = idea.Opportunity ?? string.Empty,
            Hypothesis = idea.Hypothesis ?? string.Empty,
            TargetAudience = idea.TargetAudience ?? string.Empty,
            ValueProposition = idea.ValueProposition ?? string.Empty,
            Constraints = idea.Constraints ?? string.Empty,
            Unknowns = idea.Unknowns ?? string.Empty,
            Evidence = idea.Evidence ?? string.Empty,
            DesiredOutcome = idea.DesiredOutcome ?? string.Empty,
            Scope = idea.Scope ?? string.Empty,
            MaturityStage = idea.MaturityStage,
            Visibility = idea.Visibility,
            Rating = idea.Rating,
            Upvotes = idea.Upvotes,
            Downvotes = idea.Downvotes,
            FollowersCount = idea.FollowersCount,
            ViewsCount = idea.ViewsCount,
            CategoryId = idea.CategoryId,
            CategoryName = idea.Category?.Name,
            AuthorName = idea.Author?.UserName ?? "Innovator",
            AuthorEmail = idea.Author?.Email,
            ForkedFromIdeaId = idea.ForkedFromIdeaId,
            ParentIdeaId = idea.ParentIdeaId,
            MergedIntoIdeaId = idea.MergedIntoIdeaId,
            Created = idea.CreationTime.DateTime,
            LastModified = idea.LastModificationTime?.DateTime,
            KnowledgeGapsCount = idea.KnowledgeGaps?.Count ?? 0,
            OpenGapsCount = idea.KnowledgeGaps?.Count(g => g.Status == KnowledgeGapStatus.Open) ?? 0,
            ExperimentsCount = idea.Experiments?.Count ?? 0,
            DecisionsCount = idea.Decisions?.Count ?? 0,
            ActionsCount = idea.Actions?.Count ?? 0,
            CompletedActionsCount = idea.Actions?.Count(a => a.Status == ActionItemStatus.Done) ?? 0,
            OutcomesCount = idea.Outcomes?.Count ?? 0,
            SubscriptionsCount = idea.Subscriptions?.Count ?? 0,
            SessionsCount = 1
        };
    }
}
