using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using MessengerApp.Models;
using MessengerApp.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

public class RegisterUserCommandHandler
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public RegisterUserCommandHandler(HttpClient httpClient, AuthService authService, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authService = authService;
        _authStateProvider = authStateProvider;
    }

    public async Task Handle(RegisterUserCommand command, NavigationManager navigationManager)
    {
        try
        {
            var requestBody = new RegisterUserCommand
            {
                Username = command.Username,
                Email = command.Email,
                Password = command.Password
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/api/Users/register", requestBody);

            if (response.IsSuccessStatusCode)
            {
				string responseBody = await response.Content.ReadAsStringAsync();
				var responseObject = JsonSerializer.Deserialize<JsonDocument>(responseBody); 
                string jwtToken = responseObject.RootElement.GetProperty("token").GetString();


				var user = await _authService.RegisterAndLoginAsync(new User { JwtToken = jwtToken });

                //((AuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(jwtToken);
                ((AuthStateProvider)_authStateProvider).SetAuthenticationState(user.User);
                navigationManager.NavigateTo("/");
            }
            else
            {
                // Respons Unsuccessful
            }
        }
        catch (Exception ex)
        {
        }
    }
}
