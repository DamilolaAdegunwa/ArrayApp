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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static Duende.IdentityServer.Models.IdentityResources;

namespace ArrayApp.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int> /*ApiAuthorizationDbContext<ApplicationUser>*/, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) 
        //: base(options, operationalStoreOptions)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    //public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    //public DbSet<ApplicationRole> ApplicationRoles { get; set; } //=> Set<ApplicationRole>();
    //public DbSet<ApplicationRole> Roles { get; set; } //=> Set<ApplicationRole>();
    //public DbSet<ApplicationRole> ApplicationRoles { get; set; } //=> Set<ApplicationRole>();

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
    public DbSet<KnowledgeGap> KnowledgeGaps => Set<KnowledgeGap>();
    public DbSet<IdeaHypothesis> Hypotheses => Set<IdeaHypothesis>();
    public DbSet<IdeaExperiment> Experiments => Set<IdeaExperiment>();
    public DbSet<IdeaDecision> Decisions => Set<IdeaDecision>();
    public DbSet<IdeaAction> Actions => Set<IdeaAction>();
    public DbSet<IdeaOutcome> Outcomes => Set<IdeaOutcome>();
    public DbSet<IdeaCanvasNode> CanvasNodes => Set<IdeaCanvasNode>();
    public DbSet<IdeaSubscription> IdeaSubscriptions => Set<IdeaSubscription>();
    public DbSet<DiscussionChannel> DiscussionChannels => Set<DiscussionChannel>();
    public DbSet<DiscussionMessage> DiscussionMessages => Set<DiscussionMessage>();
    public DbSet<AIAgentInsight> AIAgentInsights => Set<AIAgentInsight>();
    public DbSet<ConnectorConfig> ConnectorConfigs => Set<ConnectorConfig>();
    public DbSet<ConnectorSyncLog> ConnectorSyncLogs => Set<ConnectorSyncLog>();
    public DbSet<UserReputation> UserReputations => Set<UserReputation>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<ProvenanceLog> ProvenanceLogs => Set<ProvenanceLog>();
    public DbSet<SessionParticipant> SessionParticipants => Set<SessionParticipant>();
    public DbSet<SessionPoll> SessionPolls => Set<SessionPoll>();
    public DbSet<SessionPollOption> SessionPollOptions => Set<SessionPollOption>();

    public DbSet<ApplicationUserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

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

        //as no key!
        //builder.Entity<Browser>().HasNoKey();

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

        builder.Entity<Idea>(entity =>
        {
            entity.HasOne(i => i.ForkedFromIdea)
                .WithMany()
                .HasForeignKey(i => i.ForkedFromIdeaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.ParentIdea)
                .WithMany()
                .HasForeignKey(i => i.ParentIdeaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.MergedIntoIdea)
                .WithMany()
                .HasForeignKey(i => i.MergedIntoIdeaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(i => i.KnowledgeGaps).WithOne(kg => kg.Idea).HasForeignKey(kg => kg.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Hypotheses).WithOne(h => h.Idea).HasForeignKey(h => h.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Experiments).WithOne(e => e.Idea).HasForeignKey(e => e.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Decisions).WithOne(d => d.Idea).HasForeignKey(d => d.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Actions).WithOne(a => a.Idea).HasForeignKey(a => a.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Outcomes).WithOne(o => o.Idea).HasForeignKey(o => o.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.CanvasNodes).WithOne(c => c.Idea).HasForeignKey(c => c.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.Subscriptions).WithOne(s => s.Idea).HasForeignKey(s => s.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.DiscussionChannels).WithOne(dc => dc.Idea).HasForeignKey(dc => dc.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.AIAgentInsights).WithOne(ai => ai.Idea).HasForeignKey(ai => ai.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.ConnectorConfigs).WithOne(cc => cc.Idea).HasForeignKey(cc => cc.IdeaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(i => i.ProvenanceLogs).WithOne(pl => pl.Idea).HasForeignKey(pl => pl.IdeaId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Session>(entity =>
        {
            entity.HasMany(s => s.Attendees).WithOne(a => a.Session).HasForeignKey(a => a.SessionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(s => s.Polls).WithOne(p => p.Session).HasForeignKey(p => p.SessionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(s => s.Decisions).WithOne(d => d.Session).HasForeignKey(d => d.SessionId).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(s => s.ExtractedActions).WithOne(a => a.Session).HasForeignKey(a => a.SessionId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<DiscussionChannel>(entity =>
        {
            entity.HasMany(dc => dc.Messages).WithOne(m => m.Channel).HasForeignKey(m => m.ChannelId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserReputation>(entity =>
        {
            entity.HasMany(ur => ur.Badges).WithOne(b => b.UserReputation).HasForeignKey(b => b.UserReputationId).OnDelete(DeleteBehavior.Cascade);
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
