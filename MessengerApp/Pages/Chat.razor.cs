using Microsoft.AspNetCore.Components;

namespace CodeBehind;

public partial class ChatPage : ComponentBase, IAsyncDisposable
{
    [Inject] private FindChatMessageQueryHandler messageQueryHandler { get; set; }
    [Inject] private ChatClient chatClient { get; set; }
    [Inject] public LocalStorageUtils _localStorageUtils { get; set; }
    [Parameter] public string ChatId { get; set; }
   

    public List<ChatMessage> messages { get; set; }
    public string messageText;
    private int currentPageIndex = 0;

    protected override async Task OnInitializedAsync()
    {
        messages = new();
        await chatClient.StartAsync(await _localStorageUtils.GetJwtTokenFromLocalStorage(), ChatId);

        // Subscribe to events
        chatClient.MessageReceived += HandleMessageReceived;
        chatClient.MessageEdited += HandleMessageEdited;
        chatClient.MessageDeleted += HandleMessageDeleted;

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
    protected async Task LoadMessages()
    {
        try
        {
            // Retrieve messages for the chat
            var loadedMessages = await messageQueryHandler.Handle(new Guid(ChatId), currentPageIndex);
            if (loadedMessages != null)
            {
                messages.InsertRange(0, loadedMessages.Reverse());
                StateHasChanged(); 
            }
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error loading messages: {ex.Message}");
        }
    }

    public async Task EditMessage(int messageId)
    {
        try
        {
            await chatClient.EditMessageAsync(messageId, messageText);
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
            await chatClient.DeleteMessageAsync(messageId);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }

    private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        messages.Add(e.Message);
        StateHasChanged();
    }

    private void HandleMessageEdited(object sender, MessageEditedEventArgs e)
    {
        var editedMessage = messages.FirstOrDefault(m => m.Id == e.MessageId);
        if (editedMessage != null)
        {
            editedMessage.Message = e.NewMessage;
            StateHasChanged();
        }
    }

    private void HandleMessageDeleted(object sender, MessageDeletedEventArgs e)
    {
        messages.RemoveAll(m => m.Id == e.MessageId);
        StateHasChanged();
    }

    public async Task LoadMoreMessages()
    {
        currentPageIndex++;
        await LoadMessages();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        chatClient.MessageReceived -= HandleMessageReceived;
        chatClient.MessageEdited -= HandleMessageEdited;
        chatClient.MessageDeleted -= HandleMessageDeleted;

        await chatClient.StopAsync();
    }
}
