using MediatR;

namespace MessengerInfrastructure.Commands;

public class JoinGroupChatCommand : IRequest
{
    public Guid GroupChatId { get; set; }
    public string UserId { get; set; }
    public JoinGroupChatCommand(Guid groupChatId, string userId)
    {
        GroupChatId = groupChatId;
        UserId = userId;
    }
}
