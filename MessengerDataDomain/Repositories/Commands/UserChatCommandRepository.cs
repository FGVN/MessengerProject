using System.Threading.Tasks;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataDomain.Repositories
{

    public class UserChatCommandRepository : IUserChatCommandRepository
    {
        private readonly MessengerDbContext _context;

        public UserChatCommandRepository(MessengerDbContext context)
        {
            _context = context;
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
}
