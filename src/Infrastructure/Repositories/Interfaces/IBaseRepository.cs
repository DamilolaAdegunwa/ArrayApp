using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Infrastructure.Repositories.Interfaces;
//public interface IBaseRepository<T> : IRepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
public interface IBaseRepository<T> : IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    //Task<(bool, T)> Add(T obj);
    //Task<(bool, T)> Update(T obj);
    //Task<T> GetByPredicateReturnFirstOrDefault(Expression<Func<T, bool>> predicate);
    //Task<List<T>> GetByPredicateReturnToList(Expression<Func<T, bool>> predicate);

    //#region commented
    ////Task<IEnumerable<T>> AddBulk(IEnumerable<T> item);
    ////Task<bool> IsItemExist(Expression<Func<T, bool>> predicate);
    ////Task<List<T>> GetByPredicateWithSelectedProperties(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector);
    ////Task<T> GetByPredicateWithSelectedPropertiesFirstOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector);
    ////Task<bool> UpdateOrInsertQuery(string query, object param = null);
    ////Task<List<T>> QueryAsync(string query, object param = null);
    //#endregion
}
