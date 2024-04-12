using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MessengerApp.Services;

public class SendMessageCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly NavigationManager _navigationManager;
    private readonly LocalStorageUtils _localStorageUtils;

    public SendMessageCommandHandler(HttpWrapper httpWrapper, NavigationManager navigationManager, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _navigationManager = navigationManager;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(SendMessageCommand command)
    {
        try
        {
            var requestBody = new SendMessageCommand
            {
                ChatId = command.ChatId,
                Message = command.Message
                // Populate other properties if needed
            };

            // Assuming SendMessageDTO is the data transfer object used in the API
            await _httpWrapper.PostAsync<SendMessageCommand, object>(
                "https://localhost:7287/api/Messages/send", requestBody, await _localStorageUtils.GetJwtTokenFromLocalStorage());

            // Redirect to the chat page or perform any other action after sending the message
            _navigationManager.NavigateTo($"/chats/{command.ChatId}");
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }
}
