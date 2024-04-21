using DataDomain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessengerInfrastructure
{
	public interface IUnitOfWork : IDisposable
	{
		DbContext Context { get; }
        ICommandRepository<TEntity> GetCommandRepository<TEntity>() where TEntity : class; 
		IQueryRepository<TEntity> GetQueryRepository<TEntity>() where TEntity : class; 
		Task SaveChangesAsync(); 
	}
}