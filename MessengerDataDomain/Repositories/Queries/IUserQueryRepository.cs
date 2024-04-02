using DataDomain.Users;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataDomain.Repositories
{
	public interface IUserQueryRepository
	{
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task<User> GetUserByIdAsync(int userId);
	}

	public class UserQueryRepository : IUserQueryRepository
	{
		private readonly MessengerDbContext _context;

		public UserQueryRepository(MessengerDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			return await _context.Users.FindAsync(userId);
		}
	}
}
