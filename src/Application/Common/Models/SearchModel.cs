using System.Collections.Generic;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Application.Common.Models;

public class SearchModel
{
    public List<Idea> Ideas { get; set; } = new();
}
