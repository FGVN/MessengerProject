using System;
using System.Net.Http;
using System.Threading.Tasks;
using MessengerApp.Services;

public class LeaveGroupChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public LeaveGroupChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(Guid groupChatId)
    {
        try
        {
            // Get the JWT token from local storage
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            // Send the request to leave the group chat
            await _httpWrapper.PostAsync<object, object>(
                $"GroupChats/leave?chatId={groupChatId}", null, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error leaving group chat: {ex.Message}");
            throw;
        }
    }
}
