using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class InnovationCampaign : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ChallengeStatement { get; set; } = string.Empty;
    public string GoalDescription { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string SponsorOrganization { get; set; } = string.Empty;
    public decimal RewardPoolAmount { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public bool IsActive { get; set; } = true;
    public string CustomFormSchemaJson { get; set; } = string.Empty;
    public string BannerImageUrl { get; set; } = string.Empty;

    public List<Idea> SubmittedIdeas { get; set; } = new();
}
