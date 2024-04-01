using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
	class UserQuery : IUserQuery
	{
		private readonly IUserQueryRepository _userRepository;

		public UserQuery(IUserQueryRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			var user = await _userRepository.GetUserByIdAsync(userId);
			return MapToUserDto(user);
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			var users = await _userRepository.GetAllUsersAsync();
			return users.Select(user => MapToUserDto(user));
		}

		private User MapToUserDto(User user) => user;
	}
}
