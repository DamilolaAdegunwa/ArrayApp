using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using ArrayApp.Infrastructure.Repositories.Interfaces;

namespace ArrayApp.Infrastructure.Persistence;
// inherit from Ardalis.Specification type
//public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
public class EfRepository<T> : RepositoryBase<T>, IBaseRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        //DbContext= dbContext;
    }
    //public ApplicationDbContext DbContext { get; }

}
