using Microsoft.AspNetCore.Components;
using MessengerApp.Models;
using MessengerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

public class LoginUserCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public LoginUserCommandHandler(HttpWrapper httpWrapper, AuthService authService, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _httpWrapper = httpWrapper;
        _authService = authService;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task Handle(LoginUserCommand command)
    {
        try
        {
            var requestBody = new LoginUserCommand
            {
                Email = command.Email,
                Password = command.Password
            };

            string jwtToken = await _httpWrapper.PostAsync<LoginUserCommand, string>("https://localhost:7287/api/Users/login", requestBody);

            var user = await _authService.RegisterAndLoginAsync(new User { JwtToken = jwtToken });

            ((AuthStateProvider)_authStateProvider).SetAuthenticatedUser(user.User, jwtToken);
            _navigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }
}
