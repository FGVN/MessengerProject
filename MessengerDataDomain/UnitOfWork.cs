using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessengerInfrastructure;
using DataDomain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessengerInfrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MessengerDbContext _context;
        public DbContext Context => _context;
        private readonly Dictionary<Type, object> _commandRepositories = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _queryRepositories = new Dictionary<Type, object>();

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
                                                        (i.GetGenericTypeDefinition() == typeof(ICommandRepository<>) ||
                                                         i.GetGenericTypeDefinition() == typeof(IQueryRepository<>))))
                .ToList();

            foreach (var repositoryType in repositoryTypes)
            {
                var interfaces = repositoryType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                (i.GetGenericTypeDefinition() == typeof(ICommandRepository<>) ||
                                 i.GetGenericTypeDefinition() == typeof(IQueryRepository<>)))
                    .ToList();

                foreach (var @interface in interfaces)
                {
                    var entityType = @interface.GetGenericArguments()[0];
                    if (@interface.GetGenericTypeDefinition() == typeof(ICommandRepository<>))
                    {
                        _commandRepositories.Add(entityType, Activator.CreateInstance(repositoryType, _context));
                    }
                    else if (@interface.GetGenericTypeDefinition() == typeof(IQueryRepository<>))
                    {
                        _queryRepositories.Add(entityType, Activator.CreateInstance(repositoryType, _context));
                    }
                }
            }
        }

        public ICommandRepository<TEntity> GetCommandRepository<TEntity>() where TEntity : class
        {
            return (ICommandRepository<TEntity>)_commandRepositories[typeof(TEntity)];
        }

        public IQueryRepository<TEntity> GetQueryRepository<TEntity>() where TEntity : class
        {
            return (IQueryRepository<TEntity>)_queryRepositories[typeof(TEntity)];
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
}
