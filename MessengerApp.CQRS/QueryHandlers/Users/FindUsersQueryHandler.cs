using Microsoft.JSInterop;

public class FindUsersQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public static int ItemsPerPage = 10;

    public FindUsersQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<UserMenuItem>> Handle(FindUsersQuery query)
    {
        try
        {

            var url = $"Users/users/search";

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

            // Pass JWT token to the API call
            return await _httpWrapper.PostAsync<FindUsersQuery, IEnumerable<UserMenuItem>>(
                url, updatedQuery, await _localStorageUtils.GetJwtTokenFromLocalStorage());
        }
        catch (Exception ex)
        {
            // Handle exception
            // For now, let's return an empty list
            return Enumerable.Empty<UserMenuItem>();
        }
    }
}
