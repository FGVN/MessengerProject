using Microsoft.AspNetCore.Components;
using MessengerApp.Models;
using MessengerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

public class RegisterUserCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public RegisterUserCommandHandler(HttpWrapper httpWrapper, AuthService authService, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _httpWrapper = httpWrapper;
        _authService = authService;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task Handle(RegisterUserCommand command)
    {
        try
        {
            var requestBody = new RegisterUserCommand
            {
                Username = command.Username,
                Email = command.Email,
                Password = command.Password
            };

            string jwtToken = await _httpWrapper.PostAsync<RegisterUserCommand, string>("https://localhost:7287/api/Users/register", requestBody);

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
