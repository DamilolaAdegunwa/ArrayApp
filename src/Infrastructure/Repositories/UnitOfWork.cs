using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Repositories.Interfaces;

namespace ArrayApp.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        IdeaBaseRepository = new EfRepository<Idea>(dbContext);
    }

    public IBaseRepository<Idea> IdeaBaseRepository { get; }

}
