using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FindUsersQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly IJSRuntime _jsRuntime; // Inject IJSRuntime here

    public static int ItemsPerPage = 10;

    public FindUsersQueryHandler(HttpWrapper httpWrapper, IJSRuntime jsRuntime)
    {
        _httpWrapper = httpWrapper;
        _jsRuntime = jsRuntime;
    }

    //With auth
    //var jwtToken = await GetJwtTokenFromLocalStorage();
    //return await _httpWrapper.PostAsync<FindUsersQuery, IEnumerable<UserMenuItem>>(url, query, jwtToken);
    public async Task<IEnumerable<UserMenuItem>> Handle(FindUsersQuery query, int PageNumber)
    {
        try
        {
            var startIndex = (PageNumber - 1) * ItemsPerPage;
            var endIndex = startIndex + ItemsPerPage - 1;

            var url = $"https://localhost:7287/api/Query/users/search";

            // Modify the query object to include propertiesToGet
            var updatedQuery = new FindUsersQuery
            {
                Query = query.Query,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection,
                From = query.From,
                To = query.To,
                PropertiesToRetrieve = query.PropertiesToRetrieve
            };

            return await _httpWrapper.PostAsync<FindUsersQuery, IEnumerable<UserMenuItem>>(url, updatedQuery);
        }
        catch (Exception ex)
        {
            // Handle exception
            // For now, let's return an empty list
            return Enumerable.Empty<UserMenuItem>();
        }
    }


    private async Task<string> GetJwtTokenFromLocalStorage()
    {
        // Retrieve JWT token from localStorage using JavaScript interop
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
    }
}
