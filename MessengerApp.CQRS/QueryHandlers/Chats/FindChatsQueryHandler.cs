﻿public class FindChatsQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public FindChatsQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<ChatMenuItem>> Handle(FindChatsQuery query)
    {
        try
        {
            var url = $"Chats/userchats/search";

            var updatedQuery = new FindChatsQuery
            {
                Query = query.Query,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection,
                From = query.From,
                To = query.To,
                PropertiesToRetrieve = query.PropertiesToRetrieve
            };

            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();
            return await _httpWrapper.PostAsync<FindChatsQuery, IEnumerable<ChatMenuItem>>(
                url, updatedQuery, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            return Enumerable.Empty<ChatMenuItem>();
        }
    }
}
