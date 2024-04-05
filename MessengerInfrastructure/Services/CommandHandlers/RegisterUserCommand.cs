using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.Services
{
    public class RegisterUserCommandHandler
    {
		private readonly IJwtTokenGenerator _tokenGen;
		private readonly UserManager<User> _manager;

		public RegisterUserCommandHandler(UserManager<User> manager, IJwtTokenGenerator tokenGen)
        {
			_tokenGen = tokenGen;
			_manager = manager;
		}

        public async Task<string> Handle(RegisterUserDTO registerUserDto)
        {
			var user = new User { UserName = registerUserDto.Username, Email = registerUserDto.Email };
			await _manager.CreateAsync(user, registerUserDto.Password);
            return _tokenGen.GenerateToken(registerUserDto.Username);
        }
    }
}
