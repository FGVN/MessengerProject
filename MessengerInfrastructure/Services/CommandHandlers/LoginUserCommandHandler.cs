using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
	public class LoginUserCommandHandler
	{
		private readonly UserManager<User> _manager;
		private readonly IJwtTokenGenerator _tokenGen;

		public LoginUserCommandHandler(UserManager<User> manager, IJwtTokenGenerator tokenGen)
		{
			_manager = manager;
			_tokenGen = tokenGen;
		}

		public async Task<string> Handle(LoginUserDTO loginUserDto)
		{
			var user = await _manager.FindByEmailAsync(loginUserDto.Email);

			if (user != null && await _manager.CheckPasswordAsync(user, loginUserDto.Password))
			{
				var jwtToken = _tokenGen.GenerateToken(user.UserName);

				return jwtToken;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
