using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Domain.Entities.SessionAggregate;
public class Session
{
    // The session's name
    public string Name { get; set; }

    // The session's description
    public string Description { get; set; }

    // The date and time the session was created
    public DateTime CreatedAt { get; set; }

    // The date and time the session was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the session
    public User Creator { get; set; }

    // The session's participants
    public List<User> Participants { get; set; }

    // The session's status (e.g. "active" or "inactive")
    public string Status { get; set; }

    // The session's ideas (if it has any)
    public List<Idea> Ideas { get; set; }

    // The session's type (e.g. "brainstorming" or "planning")
    public string Type { get; set; }

    // The session's duration (if it has a fixed duration)
    public TimeSpan Duration { get; set; }
}