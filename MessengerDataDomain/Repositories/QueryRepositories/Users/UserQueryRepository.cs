using DataDomain.Users;
using MessengerInfrastructure;
using System.Linq.Expressions;

namespace DataDomain.Repositories;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly MessengerDbContext _context;

    public UserQueryRepository(MessengerDbContext context)
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
}
