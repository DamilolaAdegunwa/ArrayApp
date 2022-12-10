using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Domain.Entities.AppAggregate;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.ChatAggregate;
using ArrayApp.Domain.Entities.CommentAggregate;
using ArrayApp.Domain.Entities.FileAggregate;
using ArrayApp.Domain.Entities.GroupAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Entities.NotificationAggregate;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Entities.SubscriptionAggregate;
using ArrayApp.Domain.Entities.TagAggregate;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DbSet<Idea> Ideas { get; }
    DbSet<Advert> Adverts { get; }
    DbSet<App> Apps { get; }
    DbSet<Category> Categories { get; }
    DbSet<Chat> Chats { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Comment> Comments { get; }
    DbSet<DataFile> DataFiles { get; }
    DbSet<UserGroup> UserGroups { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Session> Sessions { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Product> Products { get; }
}
