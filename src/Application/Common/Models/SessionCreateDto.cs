using System;

namespace ArrayApp.Application.Common.Models;

public class SessionCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Brainstorm";
    public int? PrimaryIdeaId { get; set; }
    public DateTimeOffset ScheduledStartTime { get; set; } = DateTimeOffset.UtcNow;
    public double DurationMinutes { get; set; } = 60;
}
