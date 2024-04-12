using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Services.DTOs;

namespace MessengerInfrastructure.Services
{
    public class DeleteChatCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(string userId, Guid chatId)
        {
            var userChatRepository = _unitOfWork.GetQueryRepository<UserChat>();

            // Check if the user is a member of the chat
            var userChat = (await userChatRepository.GetAllAsync(uc => uc.UserId == userId && uc.ChatId == chatId)).FirstOrDefault();

            if (userChat == null)
            {
                // User is not a member of the chat
                return false;
            }

            // User is a member of the chat, proceed with deletion
            var chatRepository = _unitOfWork.GetCommandRepository<UserChat>();

            // Delete the chat
            await chatRepository.DeleteAsync(userChat);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
