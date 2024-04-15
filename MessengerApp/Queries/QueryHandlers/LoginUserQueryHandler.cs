using Microsoft.AspNetCore.Components;
using MessengerApp.Models;
using MessengerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

public class LoginUserQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public LoginUserQueryHandler(HttpWrapper httpWrapper, AuthService authService, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _httpWrapper = httpWrapper;
        _authService = authService;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task Handle(LoginUserQuery command)
    {
        try
        {
            var requestBody = new LoginUserQuery
            {
                Email = command.Email,
                Password = command.Password
            };

            string token = (await _httpWrapper.PostAsync<LoginUserQuery, TokenResponse>("https://localhost:7287/api/Users/login", requestBody)).Token;

            var user = await _authService.RegisterAndLoginAsync(new User { JwtToken = token });

            await ((AuthStateProvider)_authStateProvider).SetAuthenticatedUserAsync(user.User, token);
            _navigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
        }
    }
}
