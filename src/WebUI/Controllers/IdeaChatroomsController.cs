using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeaChatroomsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IAIAgentService _aiAgentService;
    private readonly IReputationService _reputationService;

    public IdeaChatroomsController(IApplicationDbContext context, IAIAgentService aiAgentService, IReputationService reputationService)
    {
        _context = context;
        _aiAgentService = aiAgentService;
        _reputationService = reputationService;
    }

    [HttpGet("{ideaId}/channels")]
    public async Task<ActionResult<List<DiscussionChannelDto>>> GetChannels(int ideaId)
    {
        var channels = await _context.DiscussionChannels
            .Where(c => c.IdeaId == ideaId)
            .Include(c => c.Messages)
            .Select(c => new DiscussionChannelDto
            {
                Id = c.Id,
                IdeaId = c.IdeaId,
                Name = c.Name,
                Description = c.Description,
                IsDefault = c.IsDefault,
                MessageCount = c.Messages.Count
            })
            .ToListAsync();

        return Ok(channels);
    }

    [HttpGet("channels/{channelId}/messages")]
    public async Task<ActionResult<List<DiscussionMessageDto>>> GetChannelMessages(int channelId)
    {
        var messages = await _context.DiscussionMessages
            .Where(m => m.ChannelId == channelId)
            .OrderBy(m => m.CreationTime)
            .Select(m => new DiscussionMessageDto
            {
                Id = m.Id,
                ChannelId = m.ChannelId,
                SenderName = m.SenderName,
                SenderUserId = m.SenderUserId,
                SenderRole = m.SenderRole,
                Content = m.Content,
                IsAiGenerated = m.IsAiGenerated,
                AiAgentType = m.AiAgentType,
                AttachmentUrl = m.AttachmentUrl,
                AttachmentName = m.AttachmentName,
                Created = m.CreationTime.DateTime
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("channels/{channelId}/messages")]
    public async Task<ActionResult<DiscussionMessageDto>> SendMessage(int channelId, [FromBody] SendDiscussionMessageDto dto)
    {
        var channel = await _context.DiscussionChannels.Include(c => c.Idea).FirstOrDefaultAsync(c => c.Id == channelId);
        if (channel == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-demo";
        var senderName = dto.SenderName ?? User.Identity?.Name ?? "Collaborator";

        var message = new DiscussionMessage
        {
            ChannelId = channelId,
            SenderUserId = userId,
            SenderName = senderName,
            SenderRole = dto.SenderRole ?? "Contributor",
            Content = dto.Content,
            AttachmentUrl = dto.AttachmentUrl,
            AttachmentName = dto.AttachmentName,
            CreationTime = DateTimeOffset.UtcNow
        };

        _context.DiscussionMessages.Add(message);
        await _context.SaveChangesAsync(default);

        await _reputationService.AwardPointsAsync(userId, 5, "Shared insight in idea chatroom");

        // Optional automated AI response
        if (dto.AskAiResponse)
        {
            var aiResponse = await _aiAgentService.AnswerMentorQuestionAsync(channel.IdeaId, dto.Content, dto.SenderRole ?? "Audience");
            var aiMessage = new DiscussionMessage
            {
                ChannelId = channelId,
                SenderUserId = "ai-assistant",
                SenderName = "Idea Mentor (AI)",
                SenderRole = "AI Mentor",
                Content = aiResponse,
                IsAiGenerated = true,
                AiAgentType = "Mentor",
                CreationTime = DateTimeOffset.UtcNow.AddSeconds(1)
            };
            _context.DiscussionMessages.Add(aiMessage);
            await _context.SaveChangesAsync(default);
        }

        return Ok(new DiscussionMessageDto
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            SenderName = message.SenderName,
            SenderUserId = message.SenderUserId,
            SenderRole = message.SenderRole,
            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl,
            AttachmentName = message.AttachmentName,
            VotesCount = message.VotesCount,
            Created = message.CreationTime.DateTime
        });
    }

    [HttpPost("messages/{messageId}/vote")]
    public async Task<IActionResult> VoteMessage(int messageId)
    {
        var message = await _context.DiscussionMessages.FindAsync(messageId);
        if (message == null) return NotFound();

        message.VotesCount += 1;
        await _context.SaveChangesAsync(default);

        return Ok(new { success = true, messageId, newVotesCount = message.VotesCount });
    }
}
