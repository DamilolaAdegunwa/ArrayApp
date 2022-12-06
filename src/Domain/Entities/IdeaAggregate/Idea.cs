using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.CommentAggregate;

namespace ArrayApp.Domain.Entities.IdeaAggregate;
public class Idea
{
    // The idea's title
    public string Title { get; set; }

    // The idea's description
    public string Description { get; set; }

    // The date and time the idea was created
    public DateTime CreatedAt { get; set; }

    // The date and time the idea was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the idea
    public User Creator { get; set; }

    // The idea's status (e.g. "pending" or "approved")
    public string Status { get; set; }

    // The idea's rating (if it has one)
    public double Rating { get; set; }

    // The idea's category (if it has one)
    public Category Category { get; set; }

    // The idea's tags (if it has any)
    public List<string> Tags { get; set; }

    // The idea's comments (if it has any)
    public List<Comment> Comments { get; set; }
}
/*
 This Idea class includes properties for storing information about the idea's title, description, creation and modification dates, creator, status, rating, category, tags, and comments. Of course, you can add or remove properties from this class based on the specific requirements of your application.
 */