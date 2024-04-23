using MediatR;
using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.CommandHandlers
{
    public class SendMessageCommand : IRequest<int>
    {
        public string SenderId { get; }
        public SendMessageDTO MessageDto { get; }

        public SendMessageCommand(string senderId, SendMessageDTO messageDto)
        {
            SenderId = senderId;
            MessageDto = messageDto;
        }
    }
}
