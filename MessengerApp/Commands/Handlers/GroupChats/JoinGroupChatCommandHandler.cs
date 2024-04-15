using System;
using System.Net.Http;
using System.Threading.Tasks;
using MessengerApp.Services;

public class JoinGroupChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public JoinGroupChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
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

            // Send the request to join the group chat
            var response = await _httpWrapper.PostAsync<object, object>(
                $"https://localhost:7287/api/GroupChats/join?chatId={groupChatId}", null, token);

        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error joining group chat: {ex.Message}");
            throw;
        }
    }
}
