using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.CommentAggregate;
public class Comment : BaseAuditableEntity, IAggregateRoot
{
    // The comment's text
    public string Text { get; set; }

    // The date and time the comment was created
    public DateTime CreatedAt { get; set; }

    // The user who created the comment
    //public ApplicationUser Creator { get; set; }

    // The comment's rating (if it has one)
    public int Rating { get; set; }

    // The comment's status (e.g. "pending" or "approved")
    public string Status { get; set; }

    //// The comment's parent (if it is a reply to another comment)
    //public List<Comment> Comments { get; set; }

    // The post, page, or other content that the comment is associated with
    public string Content { get; set; }
}
//public class Content
//{
//    public string Text { get; set; }
//}