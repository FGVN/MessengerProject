using DataDomain.Users;

namespace MessengerInfrastructure
{
    interface IUserQueryRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
    }

    class UserQueryRepository : IUserQueryRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
