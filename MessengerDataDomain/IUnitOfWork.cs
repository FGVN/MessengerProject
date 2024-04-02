
using DataDomain.Repositories;

namespace MessengerInfrastructure
{
	public interface IUnitOfWork : IDisposable
	{
		ICommandRepository<TEntity> GetCommandRepository<TEntity>() where TEntity : class; 
		IQueryRepository<TEntity> GetQueryRepository<TEntity>() where TEntity : class; 
		Task SaveChangesAsync(); 
	}
}