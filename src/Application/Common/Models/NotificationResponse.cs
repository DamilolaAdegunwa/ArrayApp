using System.Collections.Generic;

namespace ArrayApp.Application.Common.Models;

public class NotificationModel
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class NotificationResponse
{
    public List<NotificationModel> Notifications { get; set; } = new();
}
