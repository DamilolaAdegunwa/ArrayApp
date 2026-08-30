using System;

namespace ArrayApp.Application.Common.Models;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Status { get; set; } = "approved";
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
