using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeBehind;
public class RegisterPage : ComponentBase
{
    [Inject] RegisterUserCommandHandler RegisterUserHandler { get; set; }
    [Inject] AuthenticationStateProvider _authStateProvider { get; set; }
    [Inject] NavigationManager navman { get; set; }
    protected string Username { get; set; }
    protected string Email { get; set; }
    protected string Password { get; set; }
    protected async Task Handle()
    {
        var command = new RegisterUserCommand
        {
            Username = Username,
            Email = Email,
            Password = Password
        };
        await RegisterUserHandler.Handle(command);
    }
}
