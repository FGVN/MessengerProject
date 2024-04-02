using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using System.Security.Cryptography;
using System.Text;

namespace MessengerInfrastructure.Services
{
	public class UserCommand : IUserCommand
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserCommand(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task DeleteUserAsync(User user)
		{
			await _unitOfWork.GetCommandRepository<User>().DeleteAsync(user);
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task RegisterUserAsync(RegisterUserDTO registerUserDto)
		{
			// Hash the password before storing it
			var hashedPassword = HashPassword(registerUserDto.Password);

			var user = new User
			{
				Username = registerUserDto.Username,
				Email = registerUserDto.Email,
				Password = hashedPassword  // Store the hashed password
			};

			await _unitOfWork.GetCommandRepository<User>().AddAsync(user);
			await _unitOfWork.SaveChangesAsync();
		}
		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				// Compute hash from the password bytes
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

				// Convert hashed bytes to hexadecimal string
				StringBuilder builder = new StringBuilder();
				foreach (byte b in hashedBytes)
				{
					builder.Append(b.ToString("x2"));
				}

				return builder.ToString();
			}
		}
	}
}
