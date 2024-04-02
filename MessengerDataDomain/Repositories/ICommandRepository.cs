using MessengerDataAccess.Repositories;

namespace DataDomain.Repositories
{
	public interface ICommandRepository<TEntity> : IRepository
	{
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
		// Add additional command methods here if needed
	}
}

