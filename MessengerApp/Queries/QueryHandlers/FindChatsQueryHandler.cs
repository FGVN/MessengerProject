using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FindChatsQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public static int ItemsPerPage = 10;

    public FindChatsQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<ChatMenuItem>> Handle(FindChatsQuery query, int PageNumber)
    {
        try
        {
            var startIndex = (PageNumber - 1) * ItemsPerPage;
            var endIndex = startIndex + ItemsPerPage - 1;

            var url = $"https://localhost:7287/api/Chats/userchats/search";

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
