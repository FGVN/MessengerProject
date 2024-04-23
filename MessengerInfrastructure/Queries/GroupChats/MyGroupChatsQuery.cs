using DataAccess.Models;
using MediatR;

namespace MessengerInfrastructure.Query;

public class MyGroupChatsQuery : IRequest<IEnumerable<GroupChat>>
{
    public string UserId { get; set; }
}
