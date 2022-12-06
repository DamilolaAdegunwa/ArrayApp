using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Domain.Entities.IdeaAggregate.Events;
public class IdeaCommentedEvent
{
}
/*
 
 Here are some examples of events that could be associated with an Idea class in C#:

IdeaCreatedNotificationHandler: This event could be raised when a new idea is created. It could include information about the idea, such as its title and description.
IdeaUpdatedNotificationHandler: This event could be raised when an existing idea is updated. It could include information about the changes that were made to the idea.
IdeaDeletedNotificationHandler: This event could be raised when an existing idea is deleted. It could include information about the idea that was deleted.
IdeaViewedNotificationHandler: This event could be raised when an existing idea is viewed by a user. It could include information about the user who viewed the idea and the date and time when it was viewed.
IdeaLikedNotificationHandler: This event could be raised when an existing idea is liked by a user. It could include information about the user who liked the idea and the date and time when it was liked.
IdeaCommentedNotificationHandler: This event could be raised when an existing idea receives a new comment. It could include information about the comment, such as its text and the user who wrote it.
IdeaModeratedNotificationHandler: This event could be raised when an existing idea is moderated (e.g. approved or rejected). It could include information about the moderation action that was taken and the user who performed it.
Of course, you can add or remove events from this list based on the specific requirements of your application.
 */