using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public AuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void SetAuthenticatedUser(ClaimsPrincipal user, string jwtToken)
    {
        SaveJwtTokenToLocalStorage(jwtToken);
        SetAuthenticationState(user);
    }

    public void SetAuthenticatedUser(ClaimsPrincipal user)
    {
        SetAuthenticationState(user);
    }

    public async Task ClearAuthenticationStateAsync()
    {
        await RemoveJwtTokenFromLocalStorage();
        SetAuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private void SetAuthenticationState(ClaimsPrincipal user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private async Task RemoveJwtTokenFromLocalStorage()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
    }

    private void SaveJwtTokenToLocalStorage(string jwtToken)
    {
        _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", jwtToken);
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
