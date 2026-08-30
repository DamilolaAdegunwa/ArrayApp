using System;

namespace ArrayApp.Application.Common.Models;

public class FileDataUploadDto
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string MimeType { get; set; } = "application/octet-stream";
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
}
