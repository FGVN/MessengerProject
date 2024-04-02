using DataDomain.Users;

namespace MessengerInfrastructure.Services.InterFaces
{
	public interface IUserQuery
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
