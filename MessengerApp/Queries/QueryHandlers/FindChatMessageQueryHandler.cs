using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class FindChatMessageQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public FindChatMessageQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils; 
    }

    public async Task<IEnumerable<ChatMessage>> Handle(Guid chatId)
    {
        try
        {
            var url = $"https://localhost:7287/api/Messages/chatmessages/search";

            var query = new FindMessagesQuery
            {
                Query = $"{chatId}", // Search by ChatId
                From = 0, // Start index
                To = int.MaxValue, // End index (retrieve all messages)
                SortBy = "senderId", // Sort by timestamp (optional)
                SortDirection = "desc" // Sort direction (optional)
            };

            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();
            var result = await _httpWrapper.PostAsync<FindMessagesQuery, IEnumerable<ChatMessage>>(
                url, query, token);
            return result;
        }
        catch (Exception ex)
        {
            // Handle exception
            return Enumerable.Empty<ChatMessage>();
        }
    }
}

