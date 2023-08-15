//using System.Data.Entity;
//using System.Data.Entity;
//using System.Data.Entity;
//using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;
using ArrayApp.Application.Common.Interfaces;
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
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static Duende.IdentityServer.Models.IdentityResources;

namespace ArrayApp.Infrastructure.Persistence;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) 
        : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<ApplicationRole> ApplicationRoles { get; set; } //=> Set<ApplicationRole>();

    public DbSet<Idea> Ideas => Set<Idea>();

    public DbSet<Advert> Adverts => Set<Advert>();
    public DbSet<App> Apps => Set<App>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<FileData> DataFiles => Set<FileData>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Test> Tests => Set<Test>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        ////(ApplicationUser) json columns
        //builder.Entity<ApplicationUser>().OwnsOne(
        //    author => author.Contact, ownedNavigationBuilder =>
        //    {
        //        ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
        //    });

        ////on comments
        //builder.Entity<Comment>()
        //        .HasOne(c => c.Parent)
        //        .WithOne(pc => pc.Parent)
        //        .HasForeignKey<Comment>(pc => pc.Id)
        //        .IsRequired()
        //        .OnDelete(DeleteBehavior.NoAction);

        ////map user (ApplicationUser) to contacts and Addresses table
        builder.Entity<ApplicationUser>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToTable(
                "Contacts"
                );
                ownedNavigationBuilder.OwnsOne(
                contactDetails => contactDetails.Address, ownedOwnedNavigationBuilder =>
                {
                    ownedOwnedNavigationBuilder.ToTable(
                "Addresses"
                );
                });
            });

        ////(ApplicationUser) to json
        //modelBuilder.Entity<ApplicationUser>().OwnsOne(
        //author => author.Contact, ownedNavigationBuilder =>
        //{
        //    ownedNavigationBuilder.ToJson();
        //    ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
        //});

        builder.Entity<Idea>().OwnsOne(
        post => post.Metadata, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.ToJson();
            ownedNavigationBuilder.OwnsMany(metadata => metadata.TopSearches);
            ownedNavigationBuilder.OwnsMany(metadata => metadata.TopGeographies);
            ownedNavigationBuilder.OwnsMany(
            metadata => metadata.Updates,
            ownedOwnedNavigationBuilder => ownedOwnedNavigationBuilder.OwnsMany(update => update.Commits));
        });

        /*
        modelBuilder.Entity<Blog>().Property(e => e.Id).UseHiLo();
        modelBuilder.Entity<Post>().Property(e => e.Id).UseHiLo();
        modelBuilder.Entity<Blog>(); use tpc mappint strategy
         */
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
    //}
}
