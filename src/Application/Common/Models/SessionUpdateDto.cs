using System;

namespace ArrayApp.Application.Common.Models;

public class SessionUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTimeOffset? ScheduledStartTime { get; set; }
}
