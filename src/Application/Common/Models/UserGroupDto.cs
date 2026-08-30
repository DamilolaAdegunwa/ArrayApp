using System;

namespace ArrayApp.Application.Common.Models;

public class UserGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Privacy { get; set; } = "public";
    public string Type { get; set; } = "discussion";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
