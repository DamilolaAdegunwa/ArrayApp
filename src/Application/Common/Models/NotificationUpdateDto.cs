using System;

namespace ArrayApp.Application.Common.Models;

public class NotificationUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "read";
}
