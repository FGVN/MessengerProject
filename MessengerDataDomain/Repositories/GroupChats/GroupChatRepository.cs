using MessengerDataAccess.Models.Chats;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class GroupChatRepository : IGroupChatRepository
{
    private readonly MessengerDbContext _context;

    public GroupChatRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<GroupChat>> GetAllAsync(Expression<Func<GroupChat, bool>> predicate)
    {
        var dbContext = _context;
        return dbContext.Set<GroupChat>().Where(predicate);
    }

    public IQueryable<GroupChat> GetAllQueryable(Expression<Func<GroupChat, bool>> predicate)
    {
        var queryable = _context.Set<GroupChat>().AsQueryable();

        return predicate != null ? queryable.Where(predicate) : queryable;
    }

    public async Task<GroupChat> GetByIdAsync(string id)
    {
        return await _context.GroupChats.FindAsync(Guid.Parse(id));
    }


    public Task AddAsync(GroupChat entity)
    {
        _context.GroupChats.Add(entity);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(GroupChat entity)
    {
        if (entity != null)
        {
            _context.GroupChats.Remove(entity);
        }
        return _context.SaveChangesAsync();
    }

    public Task UpdateAsync(GroupChat entity)
    {
        _context.GroupChats.Update(entity);
        return _context.SaveChangesAsync();
    }
}

