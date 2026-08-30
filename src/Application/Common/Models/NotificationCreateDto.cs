using System;

namespace ArrayApp.Application.Common.Models;

public class NotificationCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = "alert";
    public string Importance { get; set; } = "normal";
    public int UserId { get; set; }
}
