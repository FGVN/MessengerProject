using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MessengerApp.Services;

public class CreateGroupChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public CreateGroupChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }
    public async Task<Guid> Handle(CreateGroupChatCommand command)
    {
        try
        {
            // Get the JWT token from local storage
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            // Send the request to create a group chat
            var response = await _httpWrapper.PostAsync<CreateGroupChatCommand, dynamic>(
                "GroupChats/create", command, token);

            // Extract the chat ID from the response
            var chatIdString = response.GetProperty("chatId").GetString();
            var chatId = Guid.Parse(chatIdString);

            return chatId;
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error creating group chat: {ex.Message}");
            throw;
        }
    }

}
