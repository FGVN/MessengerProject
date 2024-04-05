using MessengerApp.Services;
using Microsoft.AspNetCore.Components;

public class LogoutUserCommandHandler
{
    private readonly AuthService _authService;
    private readonly AuthStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public LogoutUserCommandHandler(AuthService authService, AuthStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _authService = authService;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task ExecuteAsync()
    {
        await _authStateProvider.ClearAuthenticationStateAsync();

        _authService.Logout();

        _navigationManager.NavigateTo("/");
    }
}
