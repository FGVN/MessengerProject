using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FindUsersQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    public static int ItemsPerPage = 10;

    public FindUsersQueryHandler(HttpWrapper httpWrapper)
    {
        _httpWrapper = httpWrapper;
    }
    public async Task<IEnumerable<UserMenuItem>> Handle(FindUsersQuery query, int PageNumber)
    {
        try
        {
            var startIndex = (PageNumber - 1) * ItemsPerPage;
            var endIndex = startIndex + ItemsPerPage - 1;

            // Construct the URL with page information
            var url = $"https://localhost:7287/api/UserMenu/users/search";

            // Make the HTTP POST request with the query object in the request body
            return await _httpWrapper.PostAsync<FindUsersQuery, IEnumerable<UserMenuItem>>(url, query);
        }
        catch (Exception ex)
        {
            // Handle exception
            // For now, let's return an empty list
            return Enumerable.Empty<UserMenuItem>();
        }
    }

}
