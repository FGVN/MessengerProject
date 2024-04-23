using MediatR;

namespace MessengerInfrastructure.Commands;

public class DeleteMessageCommand : IRequest<Unit>
{
    public string SenderId { get; }
    public int MessageId { get; }

    public DeleteMessageCommand(string senderId, int messageId)
    {
        SenderId = senderId;
        MessageId = messageId;
    }
}
