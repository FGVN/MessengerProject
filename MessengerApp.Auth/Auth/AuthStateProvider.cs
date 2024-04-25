using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public AuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetAuthenticatedUserAsync(ClaimsPrincipal user, string jwtToken)
    {
        await SaveJwtTokenToLocalStorage(jwtToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearAuthenticationStateAsync()
    {
        await RemoveJwtTokenFromLocalStorage();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task RemoveJwtTokenFromLocalStorage()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
    }

    private async Task SaveJwtTokenToLocalStorage(string jwtToken)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", jwtToken);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string storedJwt = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");

        if (!string.IsNullOrEmpty(storedJwt))
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(storedJwt), "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        else
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        return new List<Claim>(); // Placeholder
    }
}
