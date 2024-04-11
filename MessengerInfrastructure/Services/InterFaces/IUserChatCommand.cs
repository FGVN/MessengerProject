using MessengerDataAccess.Models.Chats;

namespace MessengerInfrastructure.Services.Interfaces
{
    public interface IUserChatCommand
    {
        Task AddUserChatAsync(UserChat userChat);
        Task UpdateUserChatAsync(UserChat userChat);
        Task DeleteUserChatAsync(UserChat userChat);
    }
}

