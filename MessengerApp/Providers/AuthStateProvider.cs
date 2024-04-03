using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

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


//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;

//namespace MessengerApp.Services
//{
//    public class AuthStateProvider : AuthenticationStateProvider
//    {
//        private readonly IJSRuntime _jsRuntime;

//        public AuthStateProvider(IJSRuntime jsRuntime)
//        {
//            _jsRuntime = jsRuntime;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");

//            if (string.IsNullOrWhiteSpace(accessToken))
//            {
//                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//            }
//            else
//            {
//                var identity = new ClaimsIdentity(new[]
//                {
//                    new Claim(ClaimTypes.Name, "username"),
//                    new Claim("access_token", accessToken)
//                }, "jwt_auth");

//                var user = new ClaimsPrincipal(identity);
//                return new AuthenticationState(user);
//            }
//        }

//        public void MarkUserAsAuthenticated(string accessToken)
//        {
//            var identity = new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.Name, "username"),
//                new Claim("access_token", accessToken)
//            }, "jwt_auth");

//            var user = new ClaimsPrincipal(identity);
//            var authenticationState = Task.FromResult(new AuthenticationState(user));
//            NotifyAuthenticationStateChanged(authenticationState);

//            // Save token to local storage
//            _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
//        }

//        public void MarkUserAsLoggedOut()
//        {
//            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
//            var authenticationState = Task.FromResult(new AuthenticationState(anonymous));
//            NotifyAuthenticationStateChanged(authenticationState);

//            // Clear token from local storage
//            _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
//        }
//    }
//}
