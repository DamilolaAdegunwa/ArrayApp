using System;

namespace ArrayApp.Application.Common.Models;

public class SubscriptionCreateDto
{
    public int UserId { get; set; }
    public int IdeaId { get; set; }
    public decimal Price { get; set; }
}
