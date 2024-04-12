using System;
using System.Linq;
using System.Threading.Tasks;
using DataDomain.Repositories;
using DataDomain.Users;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Services.DTOs;

namespace MessengerInfrastructure.Services
{
    public class CreateChatCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(string senderId, string contactUsername)
        {
            // Get the sender's user ID from the JWT token
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var sender = (await userRepository.GetAllAsync(u => u.Id == senderId)).FirstOrDefault();
            if (sender == null)
            {
                throw new Exception("Sender not found.");
            }

            // Get the contact user by username
            var contactUser = (await userRepository.GetAllAsync(u => u.UserName == contactUsername)).FirstOrDefault();
            if (contactUser == null)
            {
                throw new Exception("Contact user not found.");
            }

            var userChatRepository = _unitOfWork.GetCommandRepository<UserChat>();

            var userChat = new UserChat
            {
                UserId = sender.Id, // Use sender's user ID
                ContactUserId = contactUser.Id // Use contact user's ID
            };

            await userChatRepository.AddAsync(userChat);
            await _unitOfWork.SaveChangesAsync();

            return userChat.ChatId;
        }
    }
}
