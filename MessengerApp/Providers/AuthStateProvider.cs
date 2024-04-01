using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class AuthStateProvider : AuthenticationStateProvider
{
	private Task<AuthenticationState> authenticationStateTask;

	public AuthStateProvider()
	{
		authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
	}

	public void SetAuthenticationState(ClaimsPrincipal user)
	{
		authenticationStateTask = Task.FromResult(new AuthenticationState(user));
		NotifyAuthenticationStateChanged(authenticationStateTask);
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		return authenticationStateTask;
	}
}


public class AuthenticatedUser
{
    public ClaimsPrincipal Principal { get; set; } = new();
}