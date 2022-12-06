using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.GroupAggregate;
public class Group
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
    public User Creator { get; set; }

    // The group's members
    public List<User> Members { get; set; }

    // The group's picture (if it has one)
    public Image Picture { get; set; }

    // The group's privacy settings (e.g. "public" or "private")
    public string Privacy { get; set; }

    // The group's type (e.g. "discussion" or "support")
    public string Type { get; set; }

    // The group's tags (if it has any)
    public List<string> Tags { get; set; }
}