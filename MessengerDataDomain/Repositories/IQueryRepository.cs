using MessengerDataAccess.Repositories;

namespace DataDomain.Repositories
{
	public interface IQueryRepository<TEntity> : IRepository
	{
		Task<IEnumerable<TEntity>> GetAllAsync();
		Task<TEntity> GetByIdAsync(int id);
		// Add additional query methods here if needed
	}
}

