using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace ArrayApp.Infrastructure.Repositories.Interfaces;
public interface IUnitOfWork
{
    IBaseRepository<Advert> AdvertBaseRepository { get; }
    IBaseRepository<App> AppBaseRepository { get; }
    IBaseRepository<Category> CategoryBaseRepository { get; }
    IBaseRepository<Chat> ChatBaseRepository { get; }
    IBaseRepository<Comment> CommentBaseRepository { get; }
    IBaseRepository<FileData> FileDataBaseRepository { get; }
    IBaseRepository<UserGroup> UserGroupBaseRepository { get; }
    IBaseRepository<Idea> IdeaBaseRepository { get; }
    IBaseRepository<Notification> NotificationBaseRepository { get; }
    IBaseRepository<Session> SessionBaseRepository { get; }
    IBaseRepository<Subscription> SubscriptionBaseRepository { get; }
    IBaseRepository<Tag> TagBaseRepository { get; }
    IBaseRepository<Product> ProductBaseRepository { get; }
}
