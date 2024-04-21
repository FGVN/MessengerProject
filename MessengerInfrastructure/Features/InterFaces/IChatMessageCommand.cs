using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.Services.Interfaces
{
    public interface IChatMessageCommand
    {
        Task AddChatMessageAsync(ChatMessage chatMessage);
        Task UpdateChatMessageAsync(ChatMessage chatMessage);
        Task DeleteChatMessageAsync(ChatMessage chatMessage);
    }
}
