using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeBehind;
public class LoginPage : ComponentBase
{
    [Inject] LoginUserQueryHandler LoginUserHandler { get; set; }
    [Inject] AuthenticationStateProvider _authStateProvider { get; set; }
    [Inject] NavigationManager navman{ get; set; }
    protected string Email { get; set; }
    protected string Password { get; set; }

    protected async Task Handle()
    {
        var command = new LoginUserQuery
        {
            Email = Email,
            Password = Password
        };
        await LoginUserHandler.Handle(command);
    }
}
