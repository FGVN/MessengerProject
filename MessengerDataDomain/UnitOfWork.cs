using System;
using System.Threading.Tasks;
using DataDomain.Users;
using DataDomain.Repositories;

namespace MessengerInfrastructure
{
	public class UnitOfWork : IDisposable
	{
		private readonly MessengerDbContext _context;

		// Repositories
		public IUserCommandRepository Users { get; }
		public IUserQueryRepository UserQueries { get; }

		public UnitOfWork(MessengerDbContext context, IUserCommandRepository usersRepository, IUserQueryRepository userQueriesRepository)
		{
			_context = context;
			Users = usersRepository;
			UserQueries = userQueriesRepository;
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
