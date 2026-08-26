using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.Common.Interfaces;

public interface IAIAgentService
{
    Task<AIAgentInsightDto> RunAgentAnalysisAsync(int ideaId, AIAgentType agentType, string? customPrompt, int? sessionId = null, CancellationToken cancellationToken = default);
    Task<string> GenerateSessionSummaryAsync(int sessionId, CancellationToken cancellationToken = default);
    Task<List<CreateActionDto>> GenerateActionBreakdownAsync(int ideaId, string decisionSummary, CancellationToken cancellationToken = default);
    Task<string> AnswerMentorQuestionAsync(int ideaId, string question, string userRole, CancellationToken cancellationToken = default);
    Task<List<DuplicateIdeaResultDto>> DetectDuplicatesAsync(string ideaTitle, string description, CancellationToken cancellationToken = default);
    Task<List<IdeaClusterDto>> ClusterIdeasAsync(CancellationToken cancellationToken = default);
    Task<SynthesizedMindMapDto> SynthesizeMindMapAsync(int sessionId, CancellationToken cancellationToken = default);
    Task<IdeaTriageResultDto> TriageIdeaAsync(int ideaId, CancellationToken cancellationToken = default);
    Task<IdeaSwotAnalysisDto> GenerateSwotAnalysisAsync(int ideaId, CancellationToken cancellationToken = default);
    Task<IdeaBotChatResponseDto> ChatWithIdeaBotAsync(int ideaId, string message, string intentMode, CancellationToken cancellationToken = default);
}

public class IdeaTriageResultDto
{
    public int IdeaId { get; set; }
    public string IdeaTitle { get; set; } = string.Empty;
    public List<string> ExtractedKeyTerms { get; set; } = new();
    public double PredictedImpactScore { get; set; }
    public string TriageCategory { get; set; } = string.Empty;
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> SuggestedActionSteps { get; set; } = new();
}

public class IdeaSwotAnalysisDto
{
    public int IdeaId { get; set; }
    public string IdeaTitle { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Opportunities { get; set; } = new();
    public List<string> Threats { get; set; } = new();
}

public class IdeaBotChatResponseDto
{
    public int IdeaId { get; set; }
    public string IntentMode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public List<string> CitationsOrPatents { get; set; } = new();
    public string? GeneratedDraftText { get; set; }
}

public class DuplicateIdeaResultDto
{
    public int ExistingIdeaId { get; set; }
    public string ExistingIdeaTitle { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

public class IdeaClusterDto
{
    public string ClusterName { get; set; } = string.Empty;
    public string ThemeDescription { get; set; } = string.Empty;
    public List<int> IdeaIds { get; set; } = new();
    public List<string> IdeaTitles { get; set; } = new();
}

public class SynthesizedMindMapDto
{
    public int SessionId { get; set; }
    public string CentralTopic { get; set; } = string.Empty;
    public List<string> ConfirmedPillars { get; set; } = new();
    public List<string> UnansweredQuestions { get; set; } = new();
    public List<IdeaCanvasNodeDto> GeneratedCanvasNodes { get; set; } = new();
}

public interface IConnectorService
{
    Task<ConnectorSyncLogDto> SyncActionAsync(int actionId, ConnectorType connectorType, CancellationToken cancellationToken = default);
    Task<bool> DispatchWebhookNotificationAsync(int ideaId, string eventType, object payload, CancellationToken cancellationToken = default);
}

public interface IReputationService
{
    Task AwardPointsAsync(string userId, int points, string reason, CancellationToken cancellationToken = default);
    Task CheckAndAwardBadgesAsync(string userId, CancellationToken cancellationToken = default);
    Task RecordIdeaContributionAsync(string userId, int ideaId, ParticipantRole role, CancellationToken cancellationToken = default);
    Task RecordActionCompletionAsync(string userId, int actionId, CancellationToken cancellationToken = default);
    Task RecordOutcomeAchievementAsync(string userId, int outcomeId, CancellationToken cancellationToken = default);
}

public interface IIdeaProductService
{
    Task<List<IdeaProductDto>> GetIdeasAsync(IdeaMaturityStage? stage = null, int? categoryId = null, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<IdeaProductDto?> GetIdeaByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IdeaProductDto> CreateIdeaAsync(CreateIdeaProductDto dto, string? userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateMaturityStageAsync(int ideaId, IdeaMaturityStage newStage, string? rationale, string? userId, CancellationToken cancellationToken = default);
    Task<IdeaProductDto> ForkIdeaAsync(int ideaId, ForkIdeaDto dto, string? userId, CancellationToken cancellationToken = default);
    Task<IdeaGraphDto> GetIdeaGraphAsync(int? focusIdeaId = null, CancellationToken cancellationToken = default);
    Task<InnovationPipelineAnalyticsDto> GetPipelineAnalyticsAsync(CancellationToken cancellationToken = default);
}
