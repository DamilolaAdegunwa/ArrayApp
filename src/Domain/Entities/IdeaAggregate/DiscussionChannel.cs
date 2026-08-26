using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class DiscussionChannel : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string Name { get; set; } = "general"; // general, research, critique, implementation, qa
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<DiscussionMessage> Messages { get; set; } = new();
}

public class DiscussionMessage : BaseAuditableEntity, IAggregateRoot
{
    public int ChannelId { get; set; }
    public DiscussionChannel? Channel { get; set; }

    public string SenderName { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public string? SenderRole { get; set; } // e.g. "Professional", "AI Researcher"
    public string Content { get; set; } = string.Empty;

    public bool IsAiGenerated { get; set; }
    public string? AiAgentType { get; set; }

    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }

    public int? ParentMessageId { get; set; }
    public DiscussionMessage? ParentMessage { get; set; }
    public List<DiscussionMessage> Replies { get; set; } = new();
}
