using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using ArrayApp.Infrastructure.Persistence;

namespace ArrayApp.Infrastructure.Repositories;
//public class BaseRepository<T> :/* DapperRepository<T>,*/ IBaseRepository<T> where T : class
//{
//    //private readonly IDbConnectionFactory _factory;
//    //private readonly IOptions<DBConnect> _dbconnection;

//    public BaseRepository(
//        //IDbConnectionFactory factory,
//        //ISqlGenerator<T> generator,
//        //IOptions<DBConnect> dbconnection
//        )
//        //: base(factory.CreateConnection(dbconnection.Value.IdentityApplicationConnection), generator)
//    {
//        //_factory = factory;
//        //_dbconnection = dbconnection;
//    }
//    protected IDbConnection GetConnection() => _factory.CreateConnection(_dbconnection.Value.IdentityApplicationConnection
//        );

//    public async Task<(bool, T)> Add(T obj)
//    {
//        if (obj == null)
//            return (false, obj);

//        try
//        {
//            var result = await InsertAsync(obj).ConfigureAwait(false);

//            return (result, obj);
//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex, $"Identity service Exception thrown while performing to add  operation on object: {JsonConvert.SerializeObject(obj)}");
//            GateforceExceptionHelper.Exception(ex);
//            throw new Exception("Error while performing Add transaction operation |", ex);
//        }


//    }
//    public async Task<IEnumerable<T>> AddBulk(IEnumerable<T> item)
//    {
//        try
//        {
//            if (item.Count() > 0)
//            {
//                int result = await BulkInsertAsync(item).ConfigureAwait(false);
//            }

//        }
//        catch (Exception ex)
//        {
//            GateforceExceptionHelper.Exception(ex);
//        }
//        return item;
//    }

//    public async Task<(bool, T)> Update(T obj)
//    {
//        if (obj == null)
//            return (false, obj);
//        try
//        {
//            var result = await UpdateAsync(obj).ConfigureAwait(false);
//            return (result, obj);
//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex, $"Identity service Exception thrown while performing update on object: {JsonConvert.SerializeObject(obj)}");
//            GateforceExceptionHelper.Exception(ex);
//            throw new Exception("Error while performing update operation |", ex);
//        }
//    }

//    public async Task<bool> IsItemExist(Expression<Func<T, bool>> predicate)
//    {
//        var item = await FindAsync(predicate).ConfigureAwait(false);
//        if (item != null)
//            return true;
//        return false;
//    }
//    public async Task<T> GetByPredicateReturnFirstOrDefault(Expression<Func<T, bool>> predicate)
//    {
//        return await FindAsync(predicate).ConfigureAwait(false);
//    }

//    public async Task<List<T>> GetByPredicateReturnToList(Expression<Func<T, bool>> predicate)
//    {
//        var item = await FindAllAsync(predicate).ConfigureAwait(false); ;
//        return item.ToList();
//    }
//    public async Task<List<T>> GetByPredicateWithSelectedProperties(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector)
//    {
//        var items = await SetSelect(selector).FindAllAsync(predicate).ConfigureAwait(false);
//        return items.ToList();
//    }
//    public async Task<T> GetByPredicateWithSelectedPropertiesFirstOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector)
//    {
//        return await SetSelect(selector).FindAsync(predicate).ConfigureAwait(false);
//    }
//    public async Task<List<T>> QueryAsync(string query, object param = null)
//    {
//        var _conn = GetConnection();
//        try
//        {
//            return (List<T>)await _conn.QueryAsync<T>(query, param).ConfigureAwait(false);
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }
//    public async Task<bool> UpdateOrInsertQuery(string query, object param = null)
//    {
//        var _conn = GetConnection();
//        try
//        {
//            await _conn.QueryAsync<T>(query, param).ConfigureAwait(false);
//            return true;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }

//}
//public class DbFactory : IDbConnectionFactory
//{
//    private readonly SqlConnectionFactory sq;

//    public DbFactory(string nameOrConnectionString)
//    {
//        sq = new SqlConnectionFactory(nameOrConnectionString);
//    }

//    public DbConnection CreateConnection(string nameOrConnectionString)
//    {
//        return sq.CreateConnection(nameOrConnectionString);
//        //throw new NotImplementedException();
//    }
//}

//--------------------------
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(bool, T)> Add(T obj)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.Execute(async () =>
        {
            using (var transactions = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Set<T>().Add(obj);
                    await _context.SaveChangesAsync();
                    await transactions.CommitAsync();
                    return (true, obj);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown while performing to add transaction operation");
                    throw new Exception("Error while performing Add transaction operation |", ex);
                }
            }
        });
    }

    public async Task<T> GetById(object Id)
    {
        return await _context.Set<T>().FindAsync(Id);
    }

    public async Task<T> GetByPredicateReturnFirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetByPredicateReturnToList(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<List<TResult>> GetByPredicateWithSelectedProperties<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
    {
        return await _context.Set<T>().Where(predicate).Select(selector).ToListAsync();
    }

    public async Task<TResult> GetByPredicateWithSelectedPropertiesFirstOrDefault<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
    {
        return await _context.Set<T>().Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public async Task LoadCollections(T item, string property)
    {
        await _context.Entry(item).Collection(property).LoadAsync();
    }

    public async Task<(bool, T)> Update(T obj)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using (var transactions = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Set<T>().Attach(obj);
                    _context.Entry(obj).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transactions.CommitAsync();
                    return (true, obj);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown while performing to update transaction operation");
                    throw new Exception("Error while performing update transaction operation |", ex);
                }
            }
        });
    }
}