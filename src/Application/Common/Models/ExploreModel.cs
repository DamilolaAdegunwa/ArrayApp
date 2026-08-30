using System.Collections.Generic;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Application.Common.Models;

public class ExploreModel
{
    public List<Idea> TrendingIdeas { get; set; } = new();
}
