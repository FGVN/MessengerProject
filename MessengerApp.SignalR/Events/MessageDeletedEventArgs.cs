public class MessageDeletedEventArgs : EventArgs
{
    public int MessageId { get; }

    public MessageDeletedEventArgs(int messageId)
    {
        MessageId = messageId;
    }
}
