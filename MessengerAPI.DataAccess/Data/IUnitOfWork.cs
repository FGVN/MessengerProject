using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;
public interface IUnitOfWork : IDisposable
{
	DbContext Context { get; }
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class; 
	Task SaveChangesAsync(); 
}