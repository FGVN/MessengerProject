using MediatR;

namespace MessengerInfrastructure.Commands;

public class EditMessageCommand : IRequest<Unit>
{
    public string SenderId { get; }
    public int MessageId { get; }
    public string NewMessage { get; }

    public EditMessageCommand(string senderId, int messageId, string newMessage)
    {
        SenderId = senderId;
        MessageId = messageId;
        NewMessage = newMessage;
    }
}
