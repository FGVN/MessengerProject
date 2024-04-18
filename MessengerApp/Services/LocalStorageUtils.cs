using Microsoft.JSInterop;

public class LocalStorageUtils
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageUtils(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetJwtTokenFromLocalStorage()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
    }
}
