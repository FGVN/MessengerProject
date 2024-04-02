using DataDomain.Repositories;


namespace MessengerInfrastructure
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly MessengerDbContext _context;
		private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();

		public UnitOfWork(MessengerDbContext context)
		{
			_context = context;
		}
		public ICommandRepository<TEntity> GetCommandRepository<TEntity>() where TEntity : class
		{
			var entityType = typeof(TEntity);
			if (!repositories.ContainsKey(entityType))
			{
				var repositoryType = typeof(ICommandRepository<>).MakeGenericType(entityType);
				var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
				repositories.Add(entityType, repositoryInstance);
			}

			return (ICommandRepository<TEntity>)repositories[entityType];
		}


		public IQueryRepository<TEntity> GetQueryRepository<TEntity>() where TEntity : class
		{
			var entityType = typeof(TEntity);
			if (!repositories.ContainsKey(entityType))
			{
				var repositoryType = typeof(IQueryRepository<>).MakeGenericType(entityType);
				var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
				repositories.Add(entityType, repositoryInstance);
			}

			return (IQueryRepository<TEntity>)repositories[entityType];
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
