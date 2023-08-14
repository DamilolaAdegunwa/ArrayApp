using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
    public string? Title { get; set; }

    // The idea's description
    public string? Description { get; set; }

    // The idea's content
    public string? Content { get; set; }

    // The idea's status (e.g. "pending" or "approved")
    public IdeaStatus Status { get; set; }

    // The idea's rating (if it has one)
    public double Rating { get; set; }

    // The idea's category (if it has one)
    public Category Category { get; set; }
    public int CategoryId { get; set; }

    // The idea's tags (if it has any)
    private List<Tag> _tags { get; set; } = new List<Tag>();
    public IEnumerable<Tag> Tags => _tags.AsReadOnly();

    // The idea's comments (if it has any)
    private List<Comment> _comments { get; set; } = new List<Comment>();
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();

    // metadata
    public PostMetadata? Metadata { get; set; }
    //the author
    //public string AuthorId { get; set; }
    //public ApplicationUser Author { get; set; }

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

public class PostMetadata
{
    public PostMetadata(int views)
    {
        Views = views;
    }

    public int Views { get; set; }

    public List<SearchTerm> TopSearches { get; } = new List<SearchTerm>();
    public List<Visits> TopGeographies { get; } = new List<Visits>();

    public List<PostUpdate> Updates { get; } = new List<PostUpdate>();
}
public class SearchTerm
{
    public SearchTerm(string term, int count)
    {
        Term = term;
        Count = count;
    }

    public string Term { get; private set; }
    public int Count { get; private set; }
}
public class Visits
{
    public Visits(double latitude, double longitude, int count)
    {
        Latitude = latitude;
        Longitude = longitude;
        Count = count;
    }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int Count { get; private set; }
    public List<string>? Browsers { get; set; }
}
public class PostUpdate
{
    public PostUpdate(IPAddress postedFrom, DateTime updatedOn)
    {
        PostedFrom = postedFrom;
        UpdatedOn = updatedOn;
    }
    public IPAddress PostedFrom { get; private set; }
    public string? UpdatedBy { get; init; }
    public DateTime UpdatedOn { get; private set; }
    public List<Commit> Commits { get; } = new();
}

public class Commit
{
    public Commit(DateTime committedOn, string comment)
    {
        CommittedOn = committedOn;
        Comment = comment;
    }
    public DateTime CommittedOn { get; private set; }
    public string Comment { get; set; }
}
public class ContactDetails
{
    public Address Address { get; set; } = null!;
    public string? Phone { get; set; }
}

public class Address
{
    public Address(string street, string city, string postcode, string country)
    {
        Street = street;
        City = city;
        Postcode = postcode;
        Country = country;
    }

    public string Street { get; set; }
    public string City { get; set; }
    public string Postcode { get; set; }
    public string Country { get; set; }
}
