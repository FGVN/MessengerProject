using MediatR;

namespace DataDomain.Users
{
    public class LoginUserDTO : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
