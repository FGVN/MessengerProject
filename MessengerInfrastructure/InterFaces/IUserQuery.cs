using DataDomain.Users;

namespace MessengerInfrastructure.Services.InterFaces
{
	public interface IUserQuery
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
