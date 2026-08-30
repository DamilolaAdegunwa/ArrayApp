using System;

namespace ArrayApp.Application.Common.Models;

public class ChatCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "group";
    public string Status { get; set; } = "active";
}
