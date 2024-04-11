using DataDomain.Users;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.Services
{
    public class LoginUserCommandHandler
    {
        private readonly IJwtTokenGenerator _tokenGen;
        private readonly UserManager<User> _manager;

        public LoginUserCommandHandler(UserManager<User> manager, IJwtTokenGenerator tokenGen)
        {
            _tokenGen = tokenGen;
            _manager = manager;
        }

        public async Task<string> Handle(LoginUserDTO loginUserDto)
        {
            var user = await _manager.FindByEmailAsync(loginUserDto.Email);
            if (user != null && await _manager.CheckPasswordAsync(user, loginUserDto.Password))
            {
                var token = _tokenGen.GenerateToken(user.Id.ToString());
                return token; 
            }
            else
            {
                return null;
            }
        }
    }
}
