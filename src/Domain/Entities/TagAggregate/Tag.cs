using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Microsoft.AspNetCore.Identity;

namespace ArrayApp.Domain.Entities.TagAggregate;
public class Tag : BaseAuditableEntity, IAggregateRoot
{
    // The name of the tag.
    [Required]
    public required string Name { get; set; }

    // The number of times the tag has been used.
    public int Count { get; set; }

    // The date and time when the tag was last used.
    public DateTimeOffset LastUsed { get; set; }

    // The user who created the tag.
    //public ApplicationUser Creator { get; set; }

    // A list of users who have used the tag.
    //public List<ApplicationUser> Users { get; set; }

    // A description of the tag and its purpose.
    public string Description { get; set; } = null!;

    // A list of related tags.
    // List<Tag> RelatedTags { get; set; }

    // A flag indicating whether the tag is active or inactive.
    public bool IsActive { get; set; }

    //list of ideas
    public List<Idea> Ideas { get; set; } = new();
}
/*
 This Tag class includes properties for the tag's name, usage count, last used date, creator, list of users, description, related tags, and active status. These properties provide information about the tag and its usage.
 */