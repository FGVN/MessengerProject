using DataAccess.Models;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class UserChatRepository : IUserChatRepository
{
    private readonly MessengerDbContext _context;

    public UserChatRepository(MessengerDbContext context)
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

    public Task AddAsync(UserChat entity)
    {
        _context.UserChats.Add(entity);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(UserChat entity)
    {
        if (entity != null)
        {
            _context.UserChats.Remove(entity);
        }
        return _context.SaveChangesAsync();
    }

    public Task UpdateAsync(UserChat entity)
    {
        _context.UserChats.Update(entity);
        return _context.SaveChangesAsync();
    }
}
