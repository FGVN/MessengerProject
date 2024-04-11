using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FindChatsQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly IJSRuntime _jsRuntime;

    public static int ItemsPerPage = 10;

    public FindChatsQueryHandler(HttpWrapper httpWrapper, IJSRuntime jsRuntime)
    {
        _httpWrapper = httpWrapper;
        _jsRuntime = jsRuntime;
    }

    public async Task<IEnumerable<ChatMenuItem>> Handle(FindChatsQuery query, int PageNumber)
    {
        try
        {
            var startIndex = (PageNumber - 1) * ItemsPerPage;
            var endIndex = startIndex + ItemsPerPage - 1;

            var url = $"https://localhost:7287/api/Query/userchats/search";

            var updatedQuery = new FindChatsQuery
            {
                Query = query.Query,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection,
                From = query.From,
                To = query.To,
                PropertiesToRetrieve = query.PropertiesToRetrieve
            };

            return await _httpWrapper.PostAsync<FindChatsQuery, IEnumerable<ChatMenuItem>>(url, updatedQuery);
        }
        catch (Exception ex)
        {
            // Handle exception
            return Enumerable.Empty<ChatMenuItem>();
        }
    }

    private async Task<UserMenuItem> GetUserById(string userId)
    {
        try
        {
            var url = $"https://localhost:7287/api/Users/{userId}"; 
            var jwtToken = await GetJwtTokenFromLocalStorage(); 
            return await _httpWrapper.GetAsync<UserMenuItem>(url);
        }
        catch (Exception ex)
        {
            // Handle exception
            return null;
        }
    }

    private async Task<string> GetJwtTokenFromLocalStorage()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
    }
}
