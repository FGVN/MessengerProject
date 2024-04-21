using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using System.Linq.Expressions;

namespace DataDomain.Repositories;

public class UserChatQueryRepository : IUserChatQueryRepository
{
    private readonly MessengerDbContext _context;

    public UserChatQueryRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<UserChat>> GetAllAsync(Expression<Func<UserChat, bool>> predicate)
    {
        var dbContext = _context;
        return dbContext.Set<UserChat>().Where(predicate);
    }

    public async Task<UserChat> GetByIdAsync(string id)
    {
        return await _context.UserChats.FindAsync(id);
    }

    public IQueryable<UserChat> GetAllQueryable(Expression<Func<UserChat, bool>> predicate)
    {
        var queryable = _context.Set<UserChat>().AsQueryable();

        return predicate != null ? queryable.Where(predicate) : queryable;
    }
}
