using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;

namespace MessengerInfrastructure.Services
{
	class UserQuery : IUserQuery
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserQuery(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			var user = await _unitOfWork.UserQueries.GetUserByIdAsync(userId);
			return user;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			var users = await _unitOfWork.UserQueries.GetAllUsersAsync();
			return users;
		}
	}
}
