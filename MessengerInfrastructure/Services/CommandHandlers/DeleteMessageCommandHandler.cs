using System;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure.Services;

namespace MessengerInfrastructure.CommandHandlers
{
    public class DeleteMessageCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(string senderId, int id)
        {
            // Is it ok?
            var queryRepo = _unitOfWork.GetQueryRepository<ChatMessage>();
            var msg = await queryRepo.GetByIdAsync(id.ToString());

            if (msg == null)
            {
                throw new ArgumentException($"Message with ID {id} not found.");
            }

            if (msg.SenderId != senderId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this message.");
            }

            var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();
            await chatMessageRepository.DeleteAsync(msg);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
