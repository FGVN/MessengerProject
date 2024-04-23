using DataDomain.Users;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MessengerDbContext _context;

    public UserRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
    {
        var dbContext = _context;
        return dbContext.Set<User>().Where(predicate);
    }

    public async Task<User> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public IQueryable<User> GetAllQueryable(Expression<Func<User, bool>> predicate)
    {
        var queryable = _context.Set<User>().AsQueryable();

        return predicate != null ? queryable.Where(predicate) : queryable;
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
