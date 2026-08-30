using System;

namespace ArrayApp.Application.Common.Models;

public class CommentCreateDto
{
    public string Text { get; set; } = string.Empty;
    public int IdeaId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public string Status { get; set; } = "approved";
}
