using DataDomain.Repositories;

namespace MessengerInfrastructure
{
	public interface IUnitOfWork : IDisposable
	{
		IUserCommandRepository Users { get; set; }
		IUserQueryRepository UserQueries { get; set; }
		Task<int> SaveChangesAsync();
	}
}

