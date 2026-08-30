using System.Collections.Generic;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Application.Common.Models;

public class UserTimelineModel
{
    public string Username { get; set; } = string.Empty;
    public List<Idea> Ideas { get; set; } = new();
}
