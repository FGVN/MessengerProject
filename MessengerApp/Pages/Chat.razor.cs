using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace CodeBehind;

public partial class ChatPage : ComponentBase, IDisposable
{
    [Inject] private FindChatMessageQueryHandler messageQueryHandler { get; set; }
    [Inject] private ChatClient chatClient { get; set; }
    [Inject] public LocalStorageUtils _localStorageUtils { get; set; }
    [Parameter] public string ChatId { get; set; }

    [Inject] public IEnumerable<ChatMessage> messages { get; set; }
    public string messageText;

    protected override async Task OnInitializedAsync()
    {
        // Start the SignalR client
        await chatClient.StartAsync(await _localStorageUtils.GetJwtTokenFromLocalStorage(), ChatId.ToString());

        // Subscribe to message received event
        chatClient.MessageReceived += HandleMessageReceived;

        // Subscribe to message edited event
        chatClient.MessageEdited += HandleMessageEdited;

        // Subscribe to message deleted event
        chatClient.MessageDeleted += HandleMessageDeleted;

        // Load initial messages
        await LoadMessages();
    }

    public async Task SendMessage()
    {
        if (!string.IsNullOrEmpty(messageText))
        {
            // Send message using the ChatClient
            var message = new SendMessageCommand { ChatId = ChatId, Message = messageText };
            await chatClient.SendMessageAsync(message);

            // Clear the message input field
            messageText = string.Empty;
        }
    }

    public async Task LoadMessages()
    {
        try
        {
            // Retrieve messages for the chat
            messages = await messageQueryHandler.Handle(new Guid(ChatId));
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error loading messages: {ex.Message}");
        }
    }

    public async Task EditMessage(int messageId, string currentMessage)
    {
        try
        {
            await chatClient.EditMessageAsync(messageId, messageText, ChatId.ToString());
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }

    public async Task DeleteMessage(int messageId)
    {
        try
        {
            await chatClient.DeleteMessageAsync(messageId, ChatId.ToString());
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }

    private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        // Convert IEnumerable to List if it's not already a List
        if (!(messages is List<ChatMessage>))
        {
            messages = messages.ToList();
        }

        // Add the received message to the list of messages
        ((List<ChatMessage>)messages).Add(e.Message);

        // Refresh UI
        StateHasChanged();
    }

    private void HandleMessageEdited(object sender, MessageEditedEventArgs e)
    {
        // Find the edited message in the list
        var editedMessage = messages.FirstOrDefault(m => m.Id == e.MessageId);

        if (editedMessage != null)
        {
            // Update the message content
            editedMessage.Message = e.NewMessage;

            // Refresh UI
            StateHasChanged();
        }
    }

    private void HandleMessageDeleted(object sender, MessageDeletedEventArgs e)
    {
        // Remove the deleted message from the list
        messages = messages.Where(m => m.Id != e.MessageId);

        // Refresh UI
        StateHasChanged();
    }

    public async void Dispose()
    {
        chatClient.MessageReceived -= HandleMessageReceived;
        chatClient.MessageEdited -= HandleMessageEdited;
        chatClient.MessageDeleted -= HandleMessageDeleted;

        await chatClient.StopAsync();
    }
}
