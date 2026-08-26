using System;
using System.Collections.Generic;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.Common.Models;

#region Knowledge Gaps & Experiments
public class KnowledgeGapDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DomainArea { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public KnowledgeGapStatus Status { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? ResolutionDetails { get; set; }
    public string? SupportingEvidenceUrl { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class CreateKnowledgeGapDto
{
    public int IdeaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DomainArea { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
}

public class ResolveKnowledgeGapDto
{
    public int GapId { get; set; }
    public string ResolutionDetails { get; set; } = string.Empty;
    public string? SupportingEvidenceUrl { get; set; }
}

public class IdeaExperimentDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int? HypothesisId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Protocol { get; set; } = string.Empty;
    public string RequiredResources { get; set; } = string.Empty;
    public string ExpectedMetric { get; set; } = string.Empty;
    public string? ActualResult { get; set; }
    public string? Learnings { get; set; }
    public ExperimentStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateExperimentDto
{
    public int IdeaId { get; set; }
    public int? HypothesisId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Protocol { get; set; } = string.Empty;
    public string RequiredResources { get; set; } = string.Empty;
    public string ExpectedMetric { get; set; } = string.Empty;
}

public class UpdateExperimentResultDto
{
    public int ExperimentId { get; set; }
    public string ActualResult { get; set; } = string.Empty;
    public string Learnings { get; set; } = string.Empty;
    public ExperimentStatus Status { get; set; } = ExperimentStatus.Validated;
}
#endregion

#region Sessions & Canvas
public class IdeaSessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SessionType SessionType { get; set; }
    public string SessionTypeName => SessionType.ToString();
    public SessionStatus SessionStatus { get; set; }
    public string SessionStatusName => SessionStatus.ToString();
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string? MeetingUrl { get; set; }
    public string? AgendaNotes { get; set; }
    public string? SharedNotes { get; set; }
    public string? AiSummary { get; set; }
    public int? PrimaryIdeaId { get; set; }
    public string? PrimaryIdeaTitle { get; set; }
    public List<SessionParticipantDto> Attendees { get; set; } = new();
    public List<IdeaCanvasNodeDto> CanvasNodes { get; set; } = new();
    public List<IdeaDecisionDto> Decisions { get; set; } = new();
    public List<IdeaActionDto> Actions { get; set; } = new();
    public List<AIAgentInsightDto> AiInsights { get; set; } = new();
}

public class CreateSessionDto
{
    public int IdeaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SessionType SessionType { get; set; } = SessionType.Brainstorm;
    public DateTime ScheduledStartTime { get; set; } = DateTime.UtcNow.AddHours(2);
    public int DurationMinutes { get; set; } = 60;
    public string? AgendaNotes { get; set; }
    public List<AIAgentType> InviteAiAgents { get; set; } = new();
}

public class SessionParticipantDto
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public string RoleName => Role.ToString();
    public bool IsHost { get; set; }
    public bool IsAiAgent { get; set; }
    public string? AiAgentType { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class JoinSessionDto
{
    public int SessionId { get; set; }
    public ParticipantRole Role { get; set; } = ParticipantRole.Audience;
    public string? DisplayName { get; set; }
}

public class IdeaCanvasNodeDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public string NodeType { get; set; } = "Sticky"; // Sticky, MindMapNode, Risk, Question, Decision, Action
    public string Content { get; set; } = string.Empty;
    public double PosX { get; set; }
    public double PosY { get; set; }
    public string ColorHex { get; set; } = "#FEF08A";
    public int VotesCount { get; set; }
    public string? AuthorName { get; set; }
    public string? ParentNodeId { get; set; }
}

public class UpdateCanvasNodeDto
{
    public int? Id { get; set; }
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public string NodeType { get; set; } = "Sticky";
    public string Content { get; set; } = string.Empty;
    public double PosX { get; set; }
    public double PosY { get; set; }
    public string ColorHex { get; set; } = "#FEF08A";
    public int VotesCount { get; set; }
    public string? AuthorName { get; set; }
    public string? ParentNodeId { get; set; }
}

public class ExtractSessionOutcomesDto
{
    public int SessionId { get; set; }
    public string SessionNotes { get; set; } = string.Empty;
    public List<CreateDecisionDto> Decisions { get; set; } = new();
    public List<CreateActionDto> Actions { get; set; } = new();
    public bool GenerateAiSummary { get; set; } = true;
}
#endregion

#region Actions & Decisions
public class IdeaActionDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string? IdeaTitle { get; set; }
    public int? SessionId { get; set; }
    public int? DecisionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OwnerUserId { get; set; }
    public string? OwnerName { get; set; }
    public string? SupportingTeam { get; set; }
    public PriorityLevel Priority { get; set; }
    public ActionItemStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Dependencies { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalReferenceKey { get; set; }
    public string? ExternalUrl { get; set; }
}

public class CreateActionDto
{
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public int? DecisionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OwnerUserId { get; set; }
    public string? SupportingTeam { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? DueDate { get; set; }
    public string? Dependencies { get; set; }
    public string? ExternalSystem { get; set; }
}

public class UpdateActionStatusDto
{
    public int ActionId { get; set; }
    public ActionItemStatus NewStatus { get; set; }
}

public class IdeaDecisionDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string? DecidedByUserName { get; set; }
    public DateTime DecidedAt { get; set; }
    public List<IdeaActionDto> ResultingActions { get; set; } = new();
}

public class CreateDecisionDto
{
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}
#endregion

#region Persistent Chatrooms
public class DiscussionChannelDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int MessageCount { get; set; }
}

public class DiscussionMessageDto
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public string? SenderRole { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsAiGenerated { get; set; }
    public string? AiAgentType { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public int VotesCount { get; set; }
    public DateTime Created { get; set; }
}

public class SendDiscussionMessageDto
{
    public int ChannelId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? SenderName { get; set; }
    public string? SenderRole { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public bool AskAiResponse { get; set; }
}
#endregion

#region AI Agents
public class AIAgentInsightDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public AIAgentType AgentType { get; set; }
    public string AgentTypeName => AgentType.ToString();
    public string AgentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FullContent { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public bool IsPinned { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class InvokeAIAgentDto
{
    public int IdeaId { get; set; }
    public int? SessionId { get; set; }
    public AIAgentType AgentType { get; set; }
    public string? CustomPrompt { get; set; }
}
#endregion

#region Connectors
public class ConnectorConfigDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public ConnectorType Type { get; set; }
    public string TypeName => Type.ToString();
    public string Name { get; set; } = string.Empty;
    public string TargetEndpoint { get; set; } = string.Empty;
    public string? ProjectOrChannelKey { get; set; }
    public bool IsActive { get; set; }
    public bool AutoSyncActions { get; set; }
    public DateTime? LastSyncTime { get; set; }
}

public class ConfigureConnectorDto
{
    public int IdeaId { get; set; }
    public ConnectorType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TargetEndpoint { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string? ProjectOrChannelKey { get; set; }
    public bool AutoSyncActions { get; set; } = true;
}

public class SyncActionToConnectorDto
{
    public int ActionId { get; set; }
    public ConnectorType ConnectorType { get; set; }
}

public class ConnectorSyncLogDto
{
    public int Id { get; set; }
    public int ConnectorConfigId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime SyncedAt { get; set; }
}
#endregion

#region Outcomes & Analytics
public class IdeaOutcomeDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public OutcomeType Type { get; set; }
    public string TypeName => Type.ToString();
    public string? ArtifactUrl { get; set; }
    public double EstimatedCostSavings { get; set; }
    public double RevenueGenerated { get; set; }
    public int ImpactedUsersCount { get; set; }
    public double EstimatedRoiPercent { get; set; }
    public string? RetrospectiveNotes { get; set; }
    public string? KeyLearnings { get; set; }
    public DateTime RealizedAt { get; set; }
}

public class RecordOutcomeDto
{
    public int IdeaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public OutcomeType Type { get; set; } = OutcomeType.Prototype;
    public string? ArtifactUrl { get; set; }
    public double EstimatedCostSavings { get; set; }
    public double RevenueGenerated { get; set; }
    public int ImpactedUsersCount { get; set; }
    public double EstimatedRoiPercent { get; set; }
    public string? RetrospectiveNotes { get; set; }
    public string? KeyLearnings { get; set; }
}

public class InnovationPipelineAnalyticsDto
{
    public int TotalIdeas { get; set; }
    public int RawCount { get; set; }
    public int ExploringCount { get; set; }
    public int StructuredCount { get; set; }
    public int ValidatingCount { get; set; }
    public int ExperimentingCount { get; set; }
    public int PlannedCount { get; set; }
    public int BuildingCount { get; set; }
    public int ImplementedCount { get; set; }
    public int MeasuredCount { get; set; }
    public int EvolvingCount { get; set; }

    public double IdeaToOutcomeConversionRate { get; set; }
    public double TotalEstimatedCostSavings { get; set; }
    public double TotalRevenueGenerated { get; set; }
    public int TotalImpactedUsers { get; set; }
    public int TotalActionsCompleted { get; set; }
    public int TotalSessionsHosted { get; set; }
    public double AverageTimeToFirstActionDays { get; set; }
}
#endregion

#region Reputation & Leaderboard
public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PrimaryReputationTitle { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int IdeasHelpedCount { get; set; }
    public int ActionsCompletedCount { get; set; }
    public int OutcomesAchievedCount { get; set; }
    public int KnowledgeGapsResolvedCount { get; set; }
    public int BadgesCount { get; set; }
    public List<UserBadgeDto> TopBadges { get; set; } = new();
}

public class UserBadgeDto
{
    public int Id { get; set; }
    public BadgeType BadgeType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
}
#endregion
