using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DbSet<Idea> Ideas { get; }
}
