using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace MessengerApp.Services
{
	public class AuthService : AuthenticationStateProvider
	{
		private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

		public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
			Task.FromResult(new AuthenticationState(currentUser));

		public async Task<AuthenticationState> RegisterAndLoginAsync(RegisterUserCommand registerCommand)
		{
			var user = await RegisterUserAsync(registerCommand);
			currentUser = user;

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));

			return new AuthenticationState(currentUser);
		}


		private async Task<ClaimsPrincipal> RegisterUserAsync(RegisterUserCommand registerCommand)
		{
			//TODO: Change with API Call before adding
			var newUserClaims = new[] {
				new Claim(ClaimTypes.Name, registerCommand.Username),
				new Claim(ClaimTypes.Email, registerCommand.Email)
            };

			var identity = new ClaimsIdentity(newUserClaims, "custom");
			var user = new ClaimsPrincipal(identity);

			var authenticatedUser = await LoginWithExternalProviderAsync(user);

			return authenticatedUser;
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
}
