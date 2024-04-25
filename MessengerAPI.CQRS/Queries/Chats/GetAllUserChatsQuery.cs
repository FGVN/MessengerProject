using DataAccess.Models;
using MediatR;

namespace MessengerInfrastructure.Query;

public class GetAllUserChatsQuery : IRequest<IEnumerable<UserChatDTO>>
{
}
