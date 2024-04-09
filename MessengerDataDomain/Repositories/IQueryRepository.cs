namespace DataDomain.Repositories
{
	public interface IQueryRepository<TEntity> 
	{
		Task<IEnumerable<TEntity>> GetAllAsync();
		Task<TEntity> GetByIdAsync(int id);
	}
}

