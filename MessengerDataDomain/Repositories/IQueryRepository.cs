using System.Linq.Expressions;

namespace DataDomain.Repositories
{
    public interface IQueryRepository<TEntity>
    {
        Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAllQueryable(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetByIdAsync(string id);
    }
}
