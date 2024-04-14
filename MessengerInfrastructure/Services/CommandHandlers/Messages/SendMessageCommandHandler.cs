using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessengerDataAccess.Models.Messages;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Services;

namespace MessengerInfrastructure.CommandHandlers
{

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

            // If it's a group chat message, update the IsGroupChat flag
            if (request.MessageDto.IsGroupChat)
            {
                chatMessage.IsGroupChat = true;
            }

            await _unitOfWork.SaveChangesAsync();

            return chatMessage.Id;
        }
    }
}
