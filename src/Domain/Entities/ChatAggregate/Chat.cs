using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.ChatAggregate;
public class Chat
{
    // The chat's name
    public string Name { get; set; }

    // The date and time the chat was created
    public DateTime CreatedAt { get; set; }

    // The date and time the chat was last modified
    public DateTime ModifiedAt { get; set; }

    // The users who are members of the chat
    public List<User> Members { get; set; }

    // The messages that have been sent in the chat
    public List<Message> Messages { get; set; }

    // The chat's type (e.g. "group" or "direct")
    public string Type { get; set; }

    // The chat's status (e.g. "active" or "inactive")
    public string Status { get; set; }

    // The chat's picture (if it has one)
    public Image Picture { get; set; }
}
