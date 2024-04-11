﻿using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.Services
{
    public class SendMessageCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(SendMessageDTO messageDto)
        {
            var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();

            var chatMessage = new ChatMessage
            {
                ChatId = messageDto.ChatId,
                SenderId = messageDto.SenderId,
                Message = messageDto.Message,
                Timestamp = DateTime.UtcNow
            };

            await chatMessageRepository.AddAsync(chatMessage);
            await _unitOfWork.SaveChangesAsync();

            return chatMessage.Id;
        }
    }
}
