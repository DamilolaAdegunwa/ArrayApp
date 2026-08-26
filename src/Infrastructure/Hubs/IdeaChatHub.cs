using System;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace ArrayApp.Infrastructure.Hubs;

public class IdeaChatHub : Hub
{
    public async Task JoinChannel(int channelId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Channel-{channelId}");
    }

    public async Task LeaveChannel(int channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Channel-{channelId}");
    }

    public async Task BroadcastMessage(int channelId, DiscussionMessageDto message)
    {
        await Clients.Group($"Channel-{channelId}").SendAsync("ReceiveChannelMessage", message);
    }
}
