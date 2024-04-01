using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;

namespace MessengerInfrastructure.Services
{
    public class RegisterUserCommandHandler
    {
        private readonly IUserCommand _userCommand;

        public RegisterUserCommandHandler(IUserCommand userCommand)
        {
            _userCommand = userCommand;
        }

        public async Task Handle(RegisterUserDTO registerUserDto)
        {
            await _userCommand.RegisterUserAsync(registerUserDto);
        }
    }
}
