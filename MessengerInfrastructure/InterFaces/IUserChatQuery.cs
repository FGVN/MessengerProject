using DataDomain.Users;
using MessengerDataAccess.Models.Chats;

namespace MessengerInfrastructure.Services.Interfaces
{
    public interface IUserChatQuery
    {
        Task<UserChat> GetUserChatByIdAsync(string userChatId);
        Task<IEnumerable<UserChat>> GetAllUserChatsAsync();
    }
}
