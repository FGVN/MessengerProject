using MediatR;

namespace MessengerInfrastructure.Query;

public class LoginUserQuery : IRequest<string> 
{
    public string Email { get; set; }
    public string Password { get; set; }
}
