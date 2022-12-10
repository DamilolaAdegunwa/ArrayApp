using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.CommentAggregate;
using ArrayApp.Domain.Entities.TagAggregate;

namespace ArrayApp.Domain.Entities.IdeaAggregate;
public class Idea : BaseAuditableEntity, IAggregateRoot
{
    public Idea()
    {
    }

    // The idea's title (topic or theme was also a name I considered!)
    public string Title { get; set; }

    // The idea's description
    public string Description { get; set; }

    // The idea's content
    public string Content { get; set; }

    // The idea's status (e.g. "pending" or "approved")
    public IdeaStatus Status { get; set; }

    // The idea's rating (if it has one)
    public double Rating { get; set; }

    // The idea's category (if it has one)
    public Category Category { get; set; }

    // The idea's tags (if it has any)
    private List<Tag> _tags { get; set; } = new List<Tag>();
    public IEnumerable<Tag> Tags => _tags.AsReadOnly();

    // The idea's comments (if it has any)
    private List<Comment> _comments { get; set; } = new List<Comment>();
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();

    public void AddTag(Tag newTag)
    {
        Guard.Against.Null(newTag, nameof(newTag));
        _tags.Add(newTag);

        var newTagAddedEvent = new NewTagAddedToIdeaEvent(this, newTag);
        AddDomainEvent(newTagAddedEvent);
    }

    public void AddComment(Comment newComment)
    {
        Guard.Against.Null(newComment, nameof(newComment));
        _comments.Add(newComment);

        var newCommentAddedEvent = new NewCommentAddedToIdeaEvent(this, newComment);
        AddDomainEvent(newCommentAddedEvent);
    }

    #region update-functions
    public void UpdateTitle(string title)
    {
        Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
    }

    public void UpdateDescription(string description)
    {
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
    }

    public void UpdateContent(string content)
    {
        Content = Guard.Against.NullOrWhiteSpace(content, nameof(content));
    }
    #endregion
}
/*
 This Idea class includes properties for storing information about the idea's title, description, creation and modification dates, creator, status, rating, category, tags, and comments. Of course, you can add or remove properties from this class based on the specific requirements of your application.
 */
public enum IdeaStatus
{
    Pending = 0,
    Approved = 1
}

#region other event models
public class NewTagAddedToIdeaEvent : BaseEvent
{
    public NewTagAddedToIdeaEvent(Idea idea, Tag tag)
    {
        Idea = idea ?? throw new ArgumentNullException(nameof(idea));
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
    }

    public Idea Idea { get; set; }
    public Tag Tag { get; set; }
    
}

public class NewCommentAddedToIdeaEvent : BaseEvent
{
    public NewCommentAddedToIdeaEvent(Idea idea, Comment comment)
    {
        Idea = idea ?? throw new ArgumentNullException(nameof(idea));
        this.comment = comment ?? throw new ArgumentNullException(nameof(comment));
    }

    public Idea Idea { get; set; }
    public Comment comment { get; set; }

}
#endregion