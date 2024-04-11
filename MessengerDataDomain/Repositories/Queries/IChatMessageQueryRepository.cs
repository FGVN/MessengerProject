using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataDomain.Repositories
{
    public interface IChatMessageQueryRepository : IQueryRepository<ChatMessage>
    {
    }
    public class ChatMessageQueryRepository : IChatMessageQueryRepository
    {
        private readonly MessengerDbContext _context;

        public ChatMessageQueryRepository(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatMessage>> GetAllAsync(Expression<Func<ChatMessage, bool>> predicate)
        {
            var dbContext = _context;
            return await dbContext.Set<ChatMessage>().Where(predicate).ToListAsync();
        }

        public async Task<ChatMessage> GetByIdAsync(string id)
        {
            return await _context.ChatMessages.FindAsync(id);
        }
    }
}
