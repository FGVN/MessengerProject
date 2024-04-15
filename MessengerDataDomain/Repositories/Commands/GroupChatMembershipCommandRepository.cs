using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;

namespace MessengerDataAccess.Repositories.Commands
{
    class GroupChatMembershipCommandRepository : IGroupChatMembershipCommandRepository
    {
        private readonly MessengerDbContext _context;

        public GroupChatMembershipCommandRepository(MessengerDbContext context)
        {
            _context = context;
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
}
