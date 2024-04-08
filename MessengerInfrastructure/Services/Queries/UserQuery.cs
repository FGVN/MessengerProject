using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;

namespace MessengerInfrastructure.Services
{
	public class UserQuery : IUserQuery
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserQuery(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			var users = await _unitOfWork.GetQueryRepository<User>().GetAllAsync();
			return users;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			var user = await _unitOfWork.GetQueryRepository<User>().GetByIdAsync(userId);
			return user;
		}
	}
}
