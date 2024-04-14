using MediatR;
using MessengerInfrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
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

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();

            var chatMessage = new ChatMessage
            {
                ChatId = request.MessageDto.ChatId,
                SenderId = request.SenderId,
                Message = request.MessageDto.Message,
                Timestamp = DateTime.UtcNow
            };

            await chatMessageRepository.AddAsync(chatMessage);
            await _unitOfWork.SaveChangesAsync();

            return chatMessage.Id;
        }
    }
}
