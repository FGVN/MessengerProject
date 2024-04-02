using System;
using System.Threading.Tasks;
using DataDomain.Users;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataDomain.Repositories
{
	public interface IUserCommandRepository
	{
		Task CreateUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task DeleteUserAsync(int userId);
	}

	public class UserRepository : IUserCommandRepository
	{
		private readonly MessengerDbContext _context;

		public UserRepository(MessengerDbContext context)
		{
			_context = context;
		}

		public async Task CreateUserAsync(User user)
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteUserAsync(int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}

		public async Task UpdateUserAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}
	}
}
