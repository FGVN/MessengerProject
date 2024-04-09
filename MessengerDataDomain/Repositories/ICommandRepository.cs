namespace DataDomain.Repositories
{
	public interface ICommandRepository<TEntity>
	{
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
	}
}

