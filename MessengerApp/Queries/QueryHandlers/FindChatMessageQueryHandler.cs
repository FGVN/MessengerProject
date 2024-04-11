using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

    class FindChatMessageQueryHandler
    {
        private readonly HttpWrapper _httpWrapper;
        private readonly IJSRuntime _jsRuntime;

        public FindChatMessageQueryHandler(HttpWrapper httpWrapper, IJSRuntime jsRuntime)
        {
            _httpWrapper = httpWrapper;
            _jsRuntime = jsRuntime;
        }

        public async Task<IEnumerable<ChatMessage>> Handle(Guid chatId)
        {
            try
            {
                var url = $"https://localhost:7287/api/Query/chatmessages/search";

                var query = new FindMessagesQuery
                {
                    Query = $"{chatId}", // Search by ChatId
                    From = 0, // Start index
                    To = int.MaxValue, // End index (retrieve all messages)
                    SortBy = "senderId", // Sort by timestamp (optional)
                    SortDirection = "desc" // Sort direction (optional)
                };

                var result = await _httpWrapper.PostAsync<FindMessagesQuery, IEnumerable<ChatMessage>>(url, query);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return Enumerable.Empty<ChatMessage>();
            }
        }

        private async Task<string> GetJwtTokenFromLocalStorage()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        }
    }

