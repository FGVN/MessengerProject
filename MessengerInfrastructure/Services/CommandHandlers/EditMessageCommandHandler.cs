using System;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.CommandHandlers
{
    public class EditMessageCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(string senderId, EditMessageDTO messageDto)
        {
            var queryRepo = _unitOfWork.GetQueryRepository<ChatMessage>();
            var msg = await queryRepo.GetByIdAsync(messageDto.Id.ToString());

            if (msg == null)
            {
                throw new ArgumentException($"Message with ID {messageDto.Id} not found.");
            }

            // Check if the sender is the author of the message
            if (msg.SenderId != senderId)
            {
                throw new UnauthorizedAccessException("You are not authorized to edit this message.");
            }

            // Update the message content
            msg.Message = messageDto.Message;

            var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();
            await chatMessageRepository.UpdateAsync(msg);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
