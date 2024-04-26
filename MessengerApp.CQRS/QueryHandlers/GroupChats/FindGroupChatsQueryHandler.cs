public class FindGroupChatsQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public FindGroupChatsQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<GroupChat>> Handle(FindGroupChatsQuery query)
    {
        try
        {
            var url = $"GroupChats/groupchats/search";

            var updatedQuery = new FindGroupChatsQuery
            {
                Query = query.Query,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection,
                From = query.From,
                To = query.To,
                PropertiesToRetrieve = query.PropertiesToRetrieve
            };

            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();
            return await _httpWrapper.PostAsync<FindGroupChatsQuery, IEnumerable<GroupChat>>(
                url, updatedQuery, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            return Enumerable.Empty<GroupChat>();
        }
    }
}