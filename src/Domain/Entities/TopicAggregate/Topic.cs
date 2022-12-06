using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.TopicAggregate;
public class Topic
{
    // The topic's name
    public string Name { get; set; }

    // The topic's description
    public string Description { get; set; }

    // The date and time the topic was created
    public DateTime CreatedAt { get; set; }

    // The date and time the topic was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the topic
    public User Creator { get; set; }

    // The topic's followers
    public List<User> Followers { get; set; }

    // The topic's posts
    public List<Post> Posts { get; set; }

    // The topic's tags (if it has any)
    public List<string> Tags { get; set; }

    // The topic's category (if it has one)
    public Category Category { get; set; }
}