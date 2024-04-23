using MediatR;

namespace MessengerInfrastructure.Query;

public class DeleteChatCommand : IRequest<bool>
{
    public string UserId { get; }
    public Guid ChatId { get; }

    public DeleteChatCommand(string userId, Guid chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }
}
