using MediatR;

namespace MessengerInfrastructure.CommandHandlers;

public class JoinGroupChatCommand : IRequest
{
    public Guid GroupChatId { get; set; }
    public string UserId { get; set; }
}
