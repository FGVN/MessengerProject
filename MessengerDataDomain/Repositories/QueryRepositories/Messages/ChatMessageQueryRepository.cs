using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure;
using System.Linq.Expressions;

namespace DataDomain.Repositories;

public class ChatMessageQueryRepository : IChatMessageQueryRepository
{
    private readonly MessengerDbContext _context;

    public ChatMessageQueryRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<ChatMessage>> GetAllAsync(Expression<Func<ChatMessage, bool>> predicate)
    {
        var dbContext = _context;
        return dbContext.Set<ChatMessage>().Where(predicate);
    }

    public IQueryable<ChatMessage> GetAllQueryable(Expression<Func<ChatMessage, bool>> predicate)
    {
        var queryable = _context.Set<ChatMessage>().AsQueryable();

        return predicate != null ? queryable.Where(predicate) : queryable;
    }

    public async Task<ChatMessage> GetByIdAsync(string id)
    {
        return await _context.ChatMessages.FindAsync(int.Parse(id));
    }
}
