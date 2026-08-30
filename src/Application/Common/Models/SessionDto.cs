using System;

namespace ArrayApp.Application.Common.Models;

public class SessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Brainstorm";
    public string Status { get; set; } = "Scheduled";
    public int? PrimaryIdeaId { get; set; }
    public DateTimeOffset ScheduledStartTime { get; set; } = DateTimeOffset.UtcNow;
    public double DurationMinutes { get; set; } = 60;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}