using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Domain.Entities.CommentAggregate.Events;
internal class CommentCreatedEvent
{
}
/*
 Here are some examples of events that could be associated with a Comment class in C#:

CommentCreatedEvent: This event is raised when a new comment is created. This could be used to add the comment to a list of comments and update the user interface to display the comment.
CommentDeletedEvent: This event is raised when a comment is deleted. This could be used to remove the comment from the list of comments and update the user interface to reflect the change.
CommentUpdatedEvent: This event is raised when a comment is updated. This could be used to update the comment in the list of comments and update the user interface to display the updated comment.
CommentReportedEvent: This event is raised when a comment is reported as inappropriate or spam. This could be used to flag the comment and take appropriate action, such as hiding it from view or removing it.
CommentModeratedEvent: This event is raised when a comment is moderated by a moderator. This could be used to update the comment's moderation status and take appropriate action, such as hiding it from view or removing it.
These are just a few examples of events that could be associated with a Comment class in C#. There may be other events available depending on the specific implementation of the Comment class.
 */