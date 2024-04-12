using MessengerApp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MessengerApp.Services;

public class AuthService
{
	private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());


	public async Task<AuthenticationState> RegisterAndLoginAsync(User user) 
	{
		var authenticatedUser = await RegisterUserAsync(user);

		currentUser = authenticatedUser;

		return new AuthenticationState(currentUser);
	}

	private async Task<ClaimsPrincipal> RegisterUserAsync(User user) 
	{

		var newUserClaims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.JwtToken), 
        };

		var identity = new ClaimsIdentity(newUserClaims, "custom");
		var registeredUser = new ClaimsPrincipal(identity);

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
    public async Task<AuthenticationState> Logout()
    {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        return new AuthenticationState(currentUser);
    }

}