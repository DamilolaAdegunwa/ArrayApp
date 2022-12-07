using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Domain.Entities.CategoryAggregate;
public class Category
{
    // The Category's name
    public string Name { get; set; }

    // The Category's description
    public string Description { get; set; }

    // The date and time the Category was created
    public DateTime CreatedAt { get; set; }

    // The date and time the Category was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the Category
    public User Creator { get; set; }

    // The Category's followers
    public List<User> Followers { get; set; }

    // The Category's posts
    public List<Idea> Posts { get; set; }

    // The Category's tags (if it has any)
    public List<string> Tags { get; set; }

    // The Category's category (if it has one)
    //public Category Category { get; set; }
}