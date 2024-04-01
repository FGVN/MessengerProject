using MessengerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

public class RegisterUserCommandHandler
{
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public RegisterUserCommandHandler(AuthService authService, AuthenticationStateProvider authStateProvider)
    {
        _authService = authService;
        _authStateProvider = authStateProvider;
    }

    public async Task Handle(RegisterUserCommand command)
    {
        var user = await _authService.RegisterAndLoginAsync(command);

        ((AuthStateProvider)_authStateProvider).SetAuthenticationState(user.User);
    }
}
