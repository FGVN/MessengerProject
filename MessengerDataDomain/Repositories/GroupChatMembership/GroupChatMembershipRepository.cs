using MessengerDataAccess.Models.Chats;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class GroupChatMembershipRepository : IGroupChatMembershipRepository
{
    private readonly MessengerDbContext _context;

    public GroupChatMembershipRepository(MessengerDbContext context)
    {
        _context = context;
    }
    public async Task<IQueryable<GroupChatMembership>> GetAllAsync(Expression<Func<GroupChatMembership, bool>> predicate)
    {
        var dbContext = _context;
        return dbContext.Set<GroupChatMembership>().Where(predicate);
    }

    public IQueryable<GroupChatMembership> GetAllQueryable(Expression<Func<GroupChatMembership, bool>> predicate)
    {
        var queryable = _context.Set<GroupChatMembership>().AsQueryable();

        return predicate != null ? queryable.Where(predicate) : queryable;
    }

    public async Task<GroupChatMembership> GetByIdAsync(string id)
    {
        return await _context.GroupChatMemberships.FindAsync(Guid.Parse(id));
    }

    public Task AddAsync(GroupChatMembership entity)
    {
        _context.GroupChatMemberships.Add(entity);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(GroupChatMembership entity)
    {
        if (entity != null)
        {
            _context.GroupChatMemberships.Remove(entity);
        }
        return _context.SaveChangesAsync();
    }

    public Task UpdateAsync(GroupChatMembership entity)
    {
        _context.GroupChatMemberships.Update(entity);
        return _context.SaveChangesAsync();
    }
}
