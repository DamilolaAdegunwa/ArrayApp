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
