using System.Threading.Tasks;
using DataDomain.Users;
using MessengerInfrastructure.Repositories;
using MessengerInfrastructure.Services.InterFaces;

namespace MessengerInfrastructure.Services
{
	public class UserCommand : IUserCommand
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserCommand(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task DeleteUserAsync(int userId)
		{
			await _unitOfWork.Users.DeleteUserAsync(userId);
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task RegisterUserAsync(RegisterUserDTO registerUserDto)
		{
			var user = new User
			{
				Username = registerUserDto.Username,
				Email = registerUserDto.Email,
				Password = registerUserDto.Password
			};

			await _unitOfWork.Users.CreateUserAsync(user);
			await _unitOfWork.SaveChangesAsync();
		}
	}
}
