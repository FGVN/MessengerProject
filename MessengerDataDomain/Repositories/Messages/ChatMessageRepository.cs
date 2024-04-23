using MessengerDataAccess.Models.Messages;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly MessengerDbContext _context;

    public ChatMessageRepository(MessengerDbContext context)
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
