using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.FileAggregate;
using ArrayApp.Domain.Entities.TagAggregate;

namespace ArrayApp.Domain.Entities.GroupAggregate;
public class UserGroup : BaseAuditableEntity, IAggregateRoot
{
    // The group's name
    public string Name { get; set; }

    // The group's description
    public string Description { get; set; }

    // The date and time the group was created
    public DateTime CreatedAt { get; set; }

    // The date and time the group was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the group
    //public ApplicationUser Creator { get; set; }

    // The group's members
    public List<ApplicationUser> Members { get; set; }

    // The group's picture (if it has one)
    public FileData Picture { get; set; }

    // The group's privacy settings (e.g. "public" or "private")
    public string Privacy { get; set; }

    // The group's type (e.g. "discussion" or "support")
    public string Type { get; set; }

    // The group's tags (if it has any)
    public List<Tag> Tags { get; set; }
}