using System;

namespace ArrayApp.Application.Common.Models;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = "alert";
    public string Importance { get; set; } = "normal";
    public string Status { get; set; } = "unread";
    public int UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
