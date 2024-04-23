using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

class ChatClient
{
    private HubConnection _connection;
    private readonly Uri _BaseUri;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    public event EventHandler<MessageEditedEventArgs> MessageEdited;
    public event EventHandler<MessageDeletedEventArgs> MessageDeleted;
    private string chatId;

    public ChatClient(Uri BaseUri)
    {
        _BaseUri = BaseUri;
    }

    public async Task StartAsync(string jwtToken, string _chatId)
    {
        try
        {
            chatId = _chatId;
            _connection = new HubConnectionBuilder()
                .WithUrl(_BaseUri+"chathub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwtToken);
                })
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

            // Start the connection
            await _connection.StartAsync();

            // Join the chat group
            await _connection.InvokeAsync("JoinChatGroup", chatId);

            Console.WriteLine("Connection started successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting connection: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(SendMessageCommand message)
    {
        try
        {
            // Send the message to the hub
            await _connection.SendAsync("SendMessage", message);
            Console.WriteLine($"Message sent: {message.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public async Task EditMessageAsync(int messageId, string newMessage)
    {
        try
        {
            await _connection.SendAsync("EditMessage", messageId, newMessage, chatId);
            Console.WriteLine($"Message {messageId} edited: {newMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }

    public async Task DeleteMessageAsync(int messageId)
    {
        try
        {
            await _connection.SendAsync("DeleteMessage", messageId, chatId);
            Console.WriteLine($"Message {messageId} deleted");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting message: {ex.Message}");
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
    public ChatMessage Message { get; }

    public MessageReceivedEventArgs(ChatMessage message)
    {
        Message = message;
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
