using DataDomain.Users;
using MessengerInfrastructure;

namespace DataDomain.Repositories;
public class UserCommandRepository : IUserCommandRepository
{
	private readonly MessengerDbContext _context;

	public UserCommandRepository(MessengerDbContext context)
	{
		_context = context;
	}

	public Task AddAsync(User entity)
	{
		_context.Users.Add(entity);
		return _context.SaveChangesAsync();
	}

	public Task DeleteAsync(User entity)
	{
		if (entity != null)
		{
			_context.Users.Remove(entity);
		}
		return _context.SaveChangesAsync();
	}

	public Task UpdateAsync(User entity)
	{
		_context.Users.Update(entity);
		return _context.SaveChangesAsync();
	}
}

