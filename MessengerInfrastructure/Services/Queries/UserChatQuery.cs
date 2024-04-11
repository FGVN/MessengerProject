using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Services.Interfaces;

namespace MessengerInfrastructure.Services
{
    public class UserChatQuery : IUserChatQuery
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserChatQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserChat>> GetAllUserChatsAsync()
        {
            var userChats = await _unitOfWork.GetQueryRepository<UserChat>().GetAllAsync(x => true);
            return userChats;
        }

        public async Task<UserChat> GetUserChatByIdAsync(string userChatId)
        {
            var userChat = await _unitOfWork.GetQueryRepository<UserChat>().GetByIdAsync(userChatId);
            return userChat;
        }
    }
}
