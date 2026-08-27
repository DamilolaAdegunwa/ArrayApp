using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.SessionAggregate;

public class Session : BaseAuditableEntity, IAggregateRoot
{
    // The session's name/topic
    public string Name { get; set; } = string.Empty;

    // The session's description / goals
    public string Description { get; set; } = string.Empty;

    // Session Type & Status
    public SessionType SessionType { get; set; } = SessionType.Brainstorm;
    public SessionStatus SessionStatus { get; set; } = SessionStatus.Scheduled;

    // Scheduled and actual timestamps
    public DateTimeOffset ScheduledStartTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ActualStartTime { get; set; }
    public DateTimeOffset? ActualEndTime { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    // Meeting link / Virtual room URL
    public string? MeetingUrl { get; set; }
    public string? AgendaNotes { get; set; }
    public string? SharedNotes { get; set; }
    public string? AiSummary { get; set; }
    public string? Transcript { get; set; }

    // Primary Idea attached to this session
    public int? PrimaryIdeaId { get; set; }
    public Idea? PrimaryIdea { get; set; }

    // Session ideas collection
    public List<Idea> Ideas { get; set; } = new();

    // Session participants
    public List<SessionParticipant> Attendees { get; set; } = new();

    // Collaborative elements
    public List<IdeaCanvasNode> CanvasNodes { get; set; } = new();
    public List<SessionPoll> Polls { get; set; } = new();
    public List<IdeaDecision> Decisions { get; set; } = new();
    public List<IdeaAction> ExtractedActions { get; set; } = new();
    public List<AIAgentInsight> AiInsights { get; set; } = new();

    // Legacy fields for backward compatibility
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "Scheduled";
    public string Type { get; set; } = "Brainstorm";
    public List<ApplicationUser> Participants { get; set; } = new();
}

public class SessionParticipant : BaseAuditableEntity, IAggregateRoot
{
    public int SessionId { get; set; }
    public Session? Session { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; } = ParticipantRole.Audience;

    public bool IsHost { get; set; }
    public bool IsAiAgent { get; set; }
    public string? AiAgentType { get; set; }

    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class SessionPoll : BaseAuditableEntity, IAggregateRoot
{
    public int SessionId { get; set; }
    public Session? Session { get; set; }

    public string Question { get; set; } = string.Empty;
    public List<SessionPollOption> Options { get; set; } = new();
    public bool IsClosed { get; set; }
}

public class SessionPollOption : BaseAuditableEntity, IAggregateRoot
{
    public int SessionPollId { get; set; }
    public SessionPoll? SessionPoll { get; set; }

    public string OptionText { get; set; } = string.Empty;
    public int VotesCount { get; set; }
}