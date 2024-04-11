using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataDomain.Repositories
{

    public class ChatMessageCommandRepository : IChatMessageCommandRepository
    {
        private readonly MessengerDbContext _context;

        public ChatMessageCommandRepository(MessengerDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(ChatMessage entity)
        {
            _context.ChatMessages.Add(entity);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(ChatMessage entity)
        {
            if (entity != null)
            {
                _context.ChatMessages.Remove(entity);
            }
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(ChatMessage entity)
        {
            _context.ChatMessages.Update(entity);
            return _context.SaveChangesAsync();
        }
    }
}


