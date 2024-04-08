using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
	public class LoginUserCommandHandler
	{
		private readonly UserManager<User> manager;
		private readonly IJwtTokenGenerator tokenGen;

		public LoginUserCommandHandler(UserManager<User> manager, IJwtTokenGenerator tokenGen)
		{
			this.manager = manager;
			this.tokenGen = tokenGen;
		}

		public async Task<string> Handle(LoginUserDTO loginUserDto)
		{
			var user = await manager.FindByEmailAsync(loginUserDto.Email);

			if (user != null && await manager.CheckPasswordAsync(user, loginUserDto.Password))
			{
				var jwtToken = tokenGen.GenerateToken(user.UserName);

				return jwtToken;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
