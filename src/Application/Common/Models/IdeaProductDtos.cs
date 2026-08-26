using System;
using System.Collections.Generic;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.Common.Models;

public class IdeaProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // Structured Dimensions
    public string ProblemStatement { get; set; } = string.Empty;
    public string Opportunity { get; set; } = string.Empty;
    public string Hypothesis { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string ValueProposition { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public string Unknowns { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
    public string DesiredOutcome { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;

    // Status & Maturity
    public IdeaMaturityStage MaturityStage { get; set; }
    public string MaturityStageName => MaturityStage.ToString();
    public IdeaVisibility Visibility { get; set; }
    public double Rating { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int FollowersCount { get; set; }
    public int ViewsCount { get; set; }

    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorEmail { get; set; }

    // Lineage
    public int? ForkedFromIdeaId { get; set; }
    public int? ParentIdeaId { get; set; }
    public int? MergedIntoIdeaId { get; set; }

    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }

    // Counts
    public int KnowledgeGapsCount { get; set; }
    public int OpenGapsCount { get; set; }
    public int ExperimentsCount { get; set; }
    public int DecisionsCount { get; set; }
    public int ActionsCount { get; set; }
    public int CompletedActionsCount { get; set; }
    public int OutcomesCount { get; set; }
    public int SubscriptionsCount { get; set; }
    public int SessionsCount { get; set; }
}

public class CreateIdeaProductDto
{
    public string Title { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string Opportunity { get; set; } = string.Empty;
    public string Hypothesis { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string ValueProposition { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public string Unknowns { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
    public string DesiredOutcome { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public IdeaVisibility Visibility { get; set; } = IdeaVisibility.Public;
    public List<string> Tags { get; set; } = new();
    public List<string> InitialKnowledgeGaps { get; set; } = new();
}

public class UpdateIdeaMaturityStageDto
{
    public IdeaMaturityStage NewStage { get; set; }
    public string? Rationale { get; set; }
}

public class ForkIdeaDto
{
    public string NewTitle { get; set; } = string.Empty;
    public string ForkReason { get; set; } = string.Empty;
}

public class IdeaGraphNodeDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ActionsCount { get; set; }
    public int OutcomesCount { get; set; }
}

public class IdeaGraphLinkDto
{
    public int Source { get; set; }
    public int Target { get; set; }
    public string Type { get; set; } = "Fork"; // Fork, ParentChild, Related, Merged
}

public class IdeaGraphDto
{
    public List<IdeaGraphNodeDto> Nodes { get; set; } = new();
    public List<IdeaGraphLinkDto> Links { get; set; } = new();
}
