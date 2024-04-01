using DataDomain.Users;
using MessengerInfrastructure.Repositories;
using MessengerInfrastructure.Services.InterFaces;

namespace MessengerInfrastructure.Services
{
	public class UserCommand : IUserCommand
	{
		private readonly IUserCommandRepository _userRepository;

		public UserCommand(IUserCommandRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task DeleteUserAsync(int userId)
		{
			await _userRepository.DeleteUserAsync(userId);
		}

		public async Task RegisterUserAsync(RegisterUserDTO registerUserDto)
		{
			var user = new User
			{
				Username = registerUserDto.Username,
				Email = registerUserDto.Email,
				Password = registerUserDto.Password
			};

			await _userRepository.CreateUserAsync(user);
		}
	}
}
