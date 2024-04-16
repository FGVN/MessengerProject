using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MessengerApp.Services;

public class UpdateGroupChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public UpdateGroupChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(UpdateGroupChatCommand command)
    {
        try
        {
            // Get the JWT token from local storage
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            // Send the request to update the group chat
            await _httpWrapper.PostAsync<UpdateGroupChatCommand, object>(
                "https://localhost:7287/api/GroupChats/update", command, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error updating group chat: {ex.Message}");
            throw;
        }
    }
}
