using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DataAccess.Repositories;
public interface IRepository<TEntity>
{
    Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetAllQueryable(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetByIdAsync(string id);

    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
