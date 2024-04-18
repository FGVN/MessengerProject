using Microsoft.AspNetCore.SignalR.Client;

class ChatClient
{
    private readonly HubConnection _connection;
    private readonly LocalStorageUtils _localStorageUtils;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    public event EventHandler<MessageEditedEventArgs> MessageEdited;
    public event EventHandler<MessageDeletedEventArgs> MessageDeleted;

    public ChatClient(LocalStorageUtils localStorageUtils)
    {
        _localStorageUtils = localStorageUtils;
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7287/chathub")
            .Build();

        // Listen for server events
        _connection.On<ChatMessage>("ReceiveMessage", message =>
        {
            OnMessageReceived(new MessageReceivedEventArgs(message));
        });

        _connection.On<int, string>("MessageEdited", (messageId, newMessage) =>
        {
            OnMessageEdited(new MessageEditedEventArgs(messageId, newMessage));
        });

        _connection.On<int>("MessageDeleted", messageId =>
        {
            OnMessageDeleted(new MessageDeletedEventArgs(messageId));
        });
    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            Console.WriteLine("Connection started successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting connection: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(SendMessageCommand message, string jwtToken)
    {
        try
        {
            // Send the message to the hub
            await _connection.SendAsync("SendMessage", message, jwtToken);
            Console.WriteLine($"Message sent: {message.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        try
        {
            await _connection.StopAsync();
            Console.WriteLine("Connection stopped successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping connection: {ex.Message}");
        }
    }

    public async Task EditMessageAsync(int messageId, string newMessage, string jwtToken)
    {
        try
        {
            await _connection.SendAsync("EditMessage", messageId, newMessage, jwtToken);
            Console.WriteLine($"Message {messageId} edited: {newMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }

    public async Task DeleteMessageAsync(int messageId, string jwtToken)
    {
        try
        {
            await _connection.SendAsync("DeleteMessage", messageId, jwtToken);
            Console.WriteLine($"Message {messageId} deleted");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }
    protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }
    protected virtual void OnMessageEdited(MessageEditedEventArgs e)
    {
        MessageEdited?.Invoke(this, e);
    }

    protected virtual void OnMessageDeleted(MessageDeletedEventArgs e)
    {
        MessageDeleted?.Invoke(this, e);
    }
}
class MessageReceivedEventArgs : EventArgs
{
    public ChatMessage _message { get; }

    public MessageReceivedEventArgs(ChatMessage message)
    {
        _message = message;
    }
}
class MessageEditedEventArgs : EventArgs
{
    public int MessageId { get; }
    public string NewMessage { get; }

    public MessageEditedEventArgs(int messageId, string newMessage)
    {
        MessageId = messageId;
        NewMessage = newMessage;
    }
}

class MessageDeletedEventArgs : EventArgs
{
    public int MessageId { get; }

    public MessageDeletedEventArgs(int messageId)
    {
        MessageId = messageId;
    }
}
