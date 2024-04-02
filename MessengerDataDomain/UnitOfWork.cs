using System;
using System.Threading.Tasks;
using DataDomain.Users;
using DataDomain.Repositories;
using System.Reflection;

namespace MessengerInfrastructure
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly MessengerDbContext _context;

		// Repositories
		public IUserCommandRepository Users { get; set; }
		public IUserQueryRepository UserQueries { get; set; }

		public UnitOfWork(MessengerDbContext context)
		{
			_context = context;

			var repositoryInterfaces = typeof(IUnitOfWork)
				.GetProperties()
				.Where(p => p.PropertyType.IsInterface && p.PropertyType.Name.EndsWith("Repository"))
				.ToList();

			foreach (var repositoryInterface in repositoryInterfaces)
			{
				var repositoryType = typeof(UnitOfWork).Assembly.GetTypes()
					.FirstOrDefault(t => repositoryInterface.PropertyType.IsAssignableFrom(t) && !t.IsInterface);

				if (repositoryType != null)
				{
					var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
					repositoryInterface.SetValue(this, repositoryInstance);
				}
				else
				{
					throw new InvalidOperationException($"No concrete implementation found for repository interface '{repositoryInterface.Name}'");
				}
			}
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

