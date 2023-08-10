using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Repositories.Interfaces;

namespace ArrayApp.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        IdeaBaseRepository = new EfRepository<Idea>(dbContext);
        AdvertBaseRepository = new EfRepository<Advert>(dbContext);
        AppBaseRepository = new EfRepository<App>(dbContext);
        CategoryBaseRepository = new EfRepository<Category>(dbContext);
        ChatBaseRepository = new EfRepository<Chat>(dbContext);
        CommentBaseRepository = new EfRepository<Comment>(dbContext);
        FileDataBaseRepository = new EfRepository<FileData>(dbContext);
        UserGroupBaseRepository = new EfRepository<UserGroup>(dbContext);
        NotificationBaseRepository = new EfRepository<Notification>(dbContext);
        SessionBaseRepository = new EfRepository<Session>(dbContext);
        SubscriptionBaseRepository = new EfRepository<Subscription>(dbContext);
        TagBaseRepository = new EfRepository<Tag>(dbContext);
        ProductBaseRepository = new EfRepository<Product>(dbContext);
    }

    public IBaseRepository<Idea> IdeaBaseRepository { get; }
    public IBaseRepository<Advert> AdvertBaseRepository { get; }
    public IBaseRepository<App> AppBaseRepository { get; }
    public IBaseRepository<Category> CategoryBaseRepository { get; }
    public IBaseRepository<Chat> ChatBaseRepository { get; }
    public IBaseRepository<Comment> CommentBaseRepository { get; }
    public IBaseRepository<FileData> FileDataBaseRepository { get; }
    public IBaseRepository<UserGroup> UserGroupBaseRepository { get; }
    public IBaseRepository<Notification> NotificationBaseRepository { get; }
    public IBaseRepository<Session> SessionBaseRepository { get; }
    public IBaseRepository<Subscription> SubscriptionBaseRepository { get; }
    public IBaseRepository<Tag> TagBaseRepository { get; }
    public IBaseRepository<Product> ProductBaseRepository { get; }
}
