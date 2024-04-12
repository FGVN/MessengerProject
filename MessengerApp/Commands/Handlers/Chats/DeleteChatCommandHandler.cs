using System;
using System.Net.Http;
using System.Threading.Tasks;
using MessengerApp.Services;

public class DeleteChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public DeleteChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(Guid chatId)
    {
        try
        {
            // Get the JWT token from local storage
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            // Send the request to delete the chat
            await _httpWrapper.DeleteAsync($"https://localhost:7287/api/Chats/deleteChat/{chatId}", token);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error deleting chat: {ex.Message}");
            throw;
        }
    }
}
