using System;

namespace ArrayApp.Application.Common.Models;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset LastUsed { get; set; } = DateTimeOffset.UtcNow;
}
