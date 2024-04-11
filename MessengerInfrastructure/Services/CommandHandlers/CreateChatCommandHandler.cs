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

        public async Task<Guid> Handle(CreateChatDTO createChatDto)
        {
            var userChatRepository = _unitOfWork.GetCommandRepository<UserChat>();

            var userChat = new UserChat
            {
                UserId = createChatDto.UserId.ToString(),
                ContactUserId = createChatDto.ContactUserId.ToString()
            };

            await userChatRepository.AddAsync(userChat);
            await _unitOfWork.SaveChangesAsync(); 

            return userChat.ChatId;
        }
    }
}
