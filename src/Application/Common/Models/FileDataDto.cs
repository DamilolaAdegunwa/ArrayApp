using System;

namespace ArrayApp.Application.Common.Models;

public class FileDataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
