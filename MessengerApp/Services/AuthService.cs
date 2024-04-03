using MessengerApp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MessengerApp.Services;

public class AuthService : AuthenticationStateProvider
{
	private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

	public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
		Task.FromResult(new AuthenticationState(currentUser));

	public async Task<AuthenticationState> RegisterAndLoginAsync(User user) // Change method signature
	{
		var authenticatedUser = await RegisterUserAsync(user);

		currentUser = authenticatedUser;

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));

		return new AuthenticationState(currentUser);
	}

	private async Task<ClaimsPrincipal> RegisterUserAsync(User user) // Change method signature
	{
		// TODO: Implement user registration logic and obtain JWT token

		var newUserClaims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.JwtToken), // Change to correct property for JWT token
            // Add other claims if necessary
        };

		var identity = new ClaimsIdentity(newUserClaims, "custom");
		var registeredUser = new ClaimsPrincipal(identity);

		// Simulate user registration and return registered user
		return await Task.FromResult(registeredUser);
	}

	private Task<ClaimsPrincipal> LoginWithExternalProviderAsync(ClaimsPrincipal user)
	{
		/*
            Provide OpenID/MSAL code to authenticate the user. See your identity 
            provider's documentation for details.

            Return a new ClaimsPrincipal based on a new ClaimsIdentity.
        */
		var authenticatedUser = new ClaimsPrincipal(user.Identity);

		return Task.FromResult(authenticatedUser);
	}

	public void Logout()
	{
		currentUser = new ClaimsPrincipal(new ClaimsIdentity());
		NotifyAuthenticationStateChanged(
			Task.FromResult(new AuthenticationState(currentUser)));
	}
}