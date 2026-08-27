#pragma warning disable
#pragma info disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.CommentAggregate;
using ArrayApp.Domain.Entities.TagAggregate;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;
public class Idea : BaseAuditableEntity, IAggregateRoot
{
    public Idea()
    {
    }

    // The idea's title (topic or theme was also a name I considered!)
    public string? Title { get; set; }

    // Short tagline / pitch
    public string? Tagline { get; set; }

    // The idea's description
    public string? Description { get; set; }

    // The idea's content (might need to activate html or not)
    public string? Content { get; set; }

    // Idea Product Structured Dimensions
    public string? ProblemStatement { get; set; }
    public string? Opportunity { get; set; }
    public string? Hypothesis { get; set; }
    public string? TargetAudience { get; set; }
    public string? ValueProposition { get; set; }
    public string? Constraints { get; set; }
    public string? Unknowns { get; set; }
    public string? Evidence { get; set; }
    public string? DesiredOutcome { get; set; }
    public string? Scope { get; set; }

    // Maturation & Life-Cycle
    public IdeaMaturityStage MaturityStage { get; set; } = IdeaMaturityStage.Raw;
    public IdeaVisibility Visibility { get; set; } = IdeaVisibility.Public;

    // The idea's status (e.g. "pending" or "approved")
    public IdeaStatus Status { get; set; }

    // The idea's rating and social signals
    public double Rating { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int FollowersCount { get; set; }
    public int ViewsCount { get; set; }

    // Idea Graph & Lineage
    public int? ForkedFromIdeaId { get; set; }
    public Idea? ForkedFromIdea { get; set; }
    public int? ParentIdeaId { get; set; }
    public Idea? ParentIdea { get; set; }
    public int? MergedIntoIdeaId { get; set; }
    public Idea? MergedIntoIdea { get; set; }

    // Innovation Campaign Link
    public int? CampaignId { get; set; }
    public InnovationCampaign? Campaign { get; set; }

    // The idea's category (if it has one)
    public Category? Category { get; set; }
    public int CategoryId { get; set; }

    // Collections
    public List<KnowledgeGap> KnowledgeGaps { get; set; } = new();
    public List<IdeaHypothesis> Hypotheses { get; set; } = new();
    public List<IdeaExperiment> Experiments { get; set; } = new();
    public List<IdeaDecision> Decisions { get; set; } = new();
    public List<IdeaAction> Actions { get; set; } = new();
    public List<IdeaOutcome> Outcomes { get; set; } = new();
    public List<IdeaCanvasNode> CanvasNodes { get; set; } = new();
    public List<IdeaSubscription> Subscriptions { get; set; } = new();
    public List<DiscussionChannel> DiscussionChannels { get; set; } = new();
    public List<AIAgentInsight> AIAgentInsights { get; set; } = new();
    public List<ConnectorConfig> ConnectorConfigs { get; set; } = new();
    public List<ProvenanceLog> ProvenanceLogs { get; set; } = new();

    // The idea's tags (if it has any)
    private List<Tag> _tags { get; set; } = new List<Tag>();
    public IEnumerable<Tag> Tags => _tags.AsReadOnly();

    // The idea's comments (if it has any)
    private List<Comment> _comments { get; set; } = new List<Comment>();
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();

    // metadata
    public PostMetadata? Metadata { get; set; }

    [ForeignKey(nameof(CreatorUserId))]
    public ApplicationUser? Author { get; set; }

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

    public void AdvanceMaturityStage(IdeaMaturityStage newStage)
    {
        MaturityStage = newStage;
    }
    #endregion
}
/*
 This Idea class includes properties for storing information about the idea's title, description, creation and modification dates, creator, status, rating, category, tags, and comments. Of course, you can add or remove properties from this class based on the specific requirements of your application.
 */
public enum IdeaStatus
{
    Pending = 0,
    Approved = 1,
    Banned = 2
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

public class PostMetadata : BaseAuditableEntity, IAggregateRoot
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
public class SearchTerm //: BaseAuditableEntity, IAggregateRoot
{
    public SearchTerm(string term, int count)
    {
        Term = term;
        Count = count;
    }

    public string Term { get; private set; }
    public int Count { get; private set; }
}
public class Visits //: BaseAuditableEntity, IAggregateRoot
{
    public Visits() { }
    public Visits(double latitude, double longitude, int count)
    {
        Latitude = latitude;
        Longitude = longitude;
        Count = count;
    }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int Count { get; private set; }
    //public List<Browser>? Browsers { get; set; }
}

public class Browser : BaseAuditableEntity, IAggregateRoot
{
    public Browser() { }
    public string Name { get; set; }
    public Visits Visits { get; set; }
    public int VisitsId { get; set; }
}

public class PostUpdate //: BaseAuditableEntity, IAggregateRoot
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

public class Commit //: BaseAuditableEntity, IAggregateRoot
{
    public Commit(DateTime committedOn, string comment)
    {
        CommittedOn = committedOn;
        Comment = comment;
    }
    public DateTime CommittedOn { get; private set; }
    public string Comment { get; set; }
}
public class ContactDetails : BaseAuditableEntity, IAggregateRoot
{
    public Address? Address { get; set; }
    public string? Phone { get; set; }
}

public class Address : BaseAuditableEntity, IAggregateRoot
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
