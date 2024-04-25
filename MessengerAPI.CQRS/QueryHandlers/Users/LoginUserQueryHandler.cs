using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Query;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.QueryHandlers;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
{
    private readonly IJwtTokenGenerator _tokenGen;
    private readonly UserManager<User> _manager;

    public LoginUserQueryHandler(UserManager<User> manager, IJwtTokenGenerator tokenGen)
    {
        _tokenGen = tokenGen;
        _manager = manager;
    }

    public async Task<string> Handle(LoginUserQuery loginUserDto, CancellationToken cancellationToken)
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
