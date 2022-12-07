using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.IdeaAggregate;

namespace ArrayApp.Infrastructure.Repositories.Interfaces;
public interface IUnitOfWork
{
    IBaseRepository<Idea> IdeaBaseRepository { get; }
}
