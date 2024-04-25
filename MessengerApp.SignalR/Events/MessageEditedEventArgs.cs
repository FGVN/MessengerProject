public class MessageEditedEventArgs : EventArgs
{
    public int MessageId { get; }
    public string NewMessage { get; }

    public MessageEditedEventArgs(int messageId, string newMessage)
    {
        MessageId = messageId;
        NewMessage = newMessage;
    }
}
