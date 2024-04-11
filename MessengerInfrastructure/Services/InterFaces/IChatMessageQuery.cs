using DataDomain.Users;
using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.Services.Interfaces
{
    public interface IChatMessageQuery
    {
        Task<ChatMessage> GetChatMessageByIdAsync(string chatMessageId);
        Task<IEnumerable<ChatMessage>> GetAllChatMessagesAsync();
    }
}
