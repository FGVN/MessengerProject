using MediatR;
using MessengerDataAccess.Models.Chats;

namespace MessengerInfrastructure.Query;

public class GetAllUserChatsQuery : IRequest<IEnumerable<UserChatDTO>>
{
}
