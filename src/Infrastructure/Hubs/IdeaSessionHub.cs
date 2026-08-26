using System;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace ArrayApp.Infrastructure.Hubs;

public class IdeaSessionHub : Hub
{
    public async Task JoinSession(int sessionId, string userName, string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        await Clients.Group($"Session-{sessionId}").SendAsync("UserJoinedSession", new
        {
            connectionId = Context.ConnectionId,
            userName,
            role,
            joinedAt = DateTime.UtcNow
        });
    }

    public async Task LeaveSession(int sessionId, string userName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        await Clients.Group($"Session-{sessionId}").SendAsync("UserLeftSession", new
        {
            connectionId = Context.ConnectionId,
            userName,
            leftAt = DateTime.UtcNow
        });
    }

    public async Task SwitchRole(int sessionId, string userName, string newRole)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("UserRoleChanged", new
        {
            userName,
            newRole,
            updatedAt = DateTime.UtcNow
        });
    }

    public async Task SendLiveMessage(int sessionId, string senderName, string senderRole, string message)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("ReceiveLiveMessage", new
        {
            senderName,
            senderRole,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task UpdateCanvasNode(int sessionId, IdeaCanvasNodeDto node)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("CanvasNodeUpdated", node);
    }

    public async Task VoteCanvasNode(int sessionId, int nodeId, int newVotesCount)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("CanvasNodeVoted", new { nodeId, newVotesCount });
    }

    public async Task DeleteCanvasNode(int sessionId, int nodeId)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("CanvasNodeDeleted", new { nodeId });
    }

    public async Task SetSpeakerState(int sessionId, string userName, string userRole, bool isSpeaking)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("SpeakerStateChanged", new
        {
            userName,
            userRole,
            isSpeaking,
            timestamp = DateTime.UtcNow
        });
    }

    #region Role-Specific Action Broadcasting
    public async Task PledgeSponsorship(int sessionId, string sponsorName, decimal amount, string resourcesDescription)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("SponsorshipPledged", new
        {
            sponsorName,
            amount,
            resourcesDescription,
            pledgedAt = DateTime.UtcNow
        });
    }

    public async Task AuthoritySignoff(int sessionId, string authorityName, string signoffType, string notes)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("AuthoritySignoffIssued", new
        {
            authorityName,
            signoffType,
            notes,
            signedAt = DateTime.UtcNow
        });
    }

    public async Task ClaimAction(int sessionId, string actionerName, int actionId, string taskTitle)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("ActionClaimed", new
        {
            actionerName,
            actionId,
            taskTitle,
            claimedAt = DateTime.UtcNow
        });
    }

    public async Task ResolveKnowledgeGapLive(int sessionId, string professionalName, int gapId, string gapTitle, string resolution)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("KnowledgeGapResolvedLive", new
        {
            professionalName,
            gapId,
            gapTitle,
            resolution,
            resolvedAt = DateTime.UtcNow
        });
    }

    public async Task AskStudentQuestion(int sessionId, string studentName, string question)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("StudentQuestionSubmitted", new
        {
            studentName,
            question,
            askedAt = DateTime.UtcNow
        });
    }

    public async Task TriggerAiFacilitator(int sessionId, string agentType, string topicPrompt, string insightSummary)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("AiFacilitatorInterjected", new
        {
            agentType,
            topicPrompt,
            insightSummary,
            generatedAt = DateTime.UtcNow
        });
    }

    public async Task CastPollVote(int sessionId, int pollId, int optionIndex, string voterName)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("PollVoteReceived", new
        {
            pollId,
            optionIndex,
            voterName,
            timestamp = DateTime.UtcNow
        });
    }
    #endregion

    public async Task BroadcastDecision(int sessionId, IdeaDecisionDto decision)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("DecisionBroadcasted", decision);
    }

    public async Task BroadcastAction(int sessionId, IdeaActionDto action)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("ActionBroadcasted", action);
    }
}
