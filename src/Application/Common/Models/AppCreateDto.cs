using System;

namespace ArrayApp.Application.Common.Models;

public class AppCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Version { get; set; } = "1.0.0";
    public string ReleaseNotes { get; set; } = string.Empty;
}
