using DataDomain.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Repositories
{
    public interface IUserCommandRepository
    {
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
    }

    public class UserRepository : IUserCommandRepository
    {
        public Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
