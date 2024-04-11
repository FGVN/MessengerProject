using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataDomain.Repositories
{
    public interface IUserChatQueryRepository : IQueryRepository<UserChat>
    {
    }

    public class UserChatQueryRepository : IUserChatQueryRepository
    {
        private readonly MessengerDbContext _context;

        public UserChatQueryRepository(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserChat>> GetAllAsync(Expression<Func<UserChat, bool>> predicate)
        {
            var dbContext = _context;
            return await dbContext.Set<UserChat>().Where(predicate).ToListAsync();
        }

        public async Task<UserChat> GetByIdAsync(string id)
        {
            return await _context.UserChats.FindAsync(id);
        }
    }
}
