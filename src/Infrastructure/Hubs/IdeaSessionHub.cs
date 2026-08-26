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

    public async Task BroadcastDecision(int sessionId, IdeaDecisionDto decision)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("DecisionBroadcasted", decision);
    }

    public async Task BroadcastAction(int sessionId, IdeaActionDto action)
    {
        await Clients.Group($"Session-{sessionId}").SendAsync("ActionBroadcasted", action);
    }
}
