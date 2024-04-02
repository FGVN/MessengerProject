using DataDomain.Users;

namespace MessengerInfrastructure.Services.InterFaces
{
	public interface IUserCommand
    {
        Task RegisterUserAsync(RegisterUserDTO registerUserDto);
        Task DeleteUserAsync(User user);
    }
}
