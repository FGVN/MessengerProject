using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure.Services.Interfaces;

namespace MessengerInfrastructure.Services
{
    public class ChatMessageQuery : IChatMessageQuery
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatMessageQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ChatMessage>> GetAllChatMessagesAsync()
        {
            var chatMessages = await _unitOfWork.GetQueryRepository<ChatMessage>().GetAllAsync(x => true);
            return chatMessages;
        }

        public async Task<ChatMessage> GetChatMessageByIdAsync(string chatMessageId)
        {
            var chatMessage = await _unitOfWork.GetQueryRepository<ChatMessage>().GetByIdAsync(chatMessageId);
            return chatMessage;
        }
    }
}
