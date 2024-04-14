using DataDomain.Repositories;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using System.Linq.Expressions;

namespace MessengerDataAccess.Repositories.Queries
{
    public interface IGroupChatQueryRepository : IQueryRepository<GroupChat>
    {
    }

    public class GroupChatQueryRepository : IGroupChatQueryRepository
    {
        private readonly MessengerDbContext _context;

        public GroupChatQueryRepository(MessengerDbContext context)
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
    }
}

