using System;

namespace ArrayApp.Application.Common.Models;

public class UserGroupCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Privacy { get; set; } = "public";
    public string Type { get; set; } = "discussion";
}
