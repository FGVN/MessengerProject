using System.Linq.Expressions;

namespace DataDomain.Repositories
{
    public interface IQueryRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetByIdAsync(string id);
    }
}
