using DataDomain.Users;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataDomain.Repositories
{
	public interface IUserQueryRepository : IQueryRepository<User>
	{
	}

	public class UserQueryRepository : IUserQueryRepository
	{
		private readonly MessengerDbContext _context;

		public UserQueryRepository(MessengerDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User> GetByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}
	}
}
