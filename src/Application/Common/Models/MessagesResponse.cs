using System.Collections.Generic;

namespace ArrayApp.Application.Common.Models;

public class MessageModel
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class MessagesResponse
{
    public List<MessageModel> Messages { get; set; } = new();
}
