using System;

namespace ArrayApp.Application.Common.Models;

public class SubscriptionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int IdeaId { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "Active";
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpirationDate { get; set; } = DateTimeOffset.UtcNow.AddMonths(1);
}
