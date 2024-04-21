using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;

namespace DataDomain.Repositories;
public class GroupChatCommandRepository : IGroupChatCommandRepository
{
    private readonly MessengerDbContext _context;

    public GroupChatCommandRepository(MessengerDbContext context)
    {
        _context = context;
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
