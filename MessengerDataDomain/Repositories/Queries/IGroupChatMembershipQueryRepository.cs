using DataDomain.Repositories;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using System.Linq.Expressions;

namespace MessengerDataAccess.Repositories.Queries
{
    interface IGroupChatMembershipQueryRepository : IQueryRepository<GroupChatMembership>
    {
    }

    public class GroupChatMembershipQueryRepository : IGroupChatMembershipQueryRepository
    {
        private readonly MessengerDbContext _context;

        public GroupChatMembershipQueryRepository(MessengerDbContext context)
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
    }
}
