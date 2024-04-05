using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MessengerApp.Models;
using MessengerApp.Services;

public class LoginUserCommandHandler
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public LoginUserCommandHandler(HttpClient httpClient, AuthService authService, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authService = authService;
        _authStateProvider = authStateProvider;
    }

    public async Task Handle(LoginUserCommand command, NavigationManager navigationManager)
    {
        try
        {
            var requestBody = new LoginUserCommand
            {
                Email = command.Email,
                Password = command.Password
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/api/Users/login", requestBody);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonDocument>(responseBody);
                string jwtToken = responseObject.RootElement.GetProperty("token").GetString();

                var user = await _authService.RegisterAndLoginAsync(new User { JwtToken = jwtToken });

                ((AuthStateProvider)_authStateProvider).SetAuthenticatedUser(user.User, jwtToken);
                navigationManager.NavigateTo("/");
            }
            else
            {
                // Response Unsuccessful
            }
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }
}
