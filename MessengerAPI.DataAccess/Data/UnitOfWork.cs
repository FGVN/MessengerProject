using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MessengerDbContext _context;
    public DbContext Context => _context;
    private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

    public UnitOfWork(MessengerDbContext context)
    {
        _context = context;
        InitializeRepositories();
    }

    private void InitializeRepositories()
    {
        var repositoryTypes = typeof(UnitOfWork).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                         t.GetInterfaces().Any(i => i.IsGenericType &&
                                                    (i.GetGenericTypeDefinition() == typeof(IRepository<>))))
            .ToList();

        foreach (var repositoryType in repositoryTypes)
        {
            var entityType = repositoryType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();

            if (entityType != null)
            {
                if (!_repositories.ContainsKey(entityType))
                {
                    var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
                    _repositories.Add(entityType, repositoryInstance);
                }
            }
        }
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var entityType = typeof(TEntity);
        if (_repositories.TryGetValue(entityType, out object repository))
        {
            return (IRepository<TEntity>)repository;
        }
        throw new ArgumentException($"Repository for entity type {entityType.Name} not found.");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
