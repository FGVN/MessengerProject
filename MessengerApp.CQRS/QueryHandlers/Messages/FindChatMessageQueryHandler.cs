public class FindChatMessageQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;
    private int _pageIndex = 0;
    private const int PageSize = 5;

    public FindChatMessageQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<ChatMessage>> Handle(Guid chatId, int pageIndex)
    {
        try
        {
            _pageIndex = pageIndex;
            var url = $"Messages/chatmessages/search";

            var query = new FindMessagesQuery
            {
                Query = $"{chatId}", // Search by ChatId
                From = _pageIndex * PageSize, // Start index for pagination
                To = (_pageIndex + 1) * PageSize - 1, // End index for pagination
                SortBy = "Timestamp", // Sort by timestamp (optional)
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
