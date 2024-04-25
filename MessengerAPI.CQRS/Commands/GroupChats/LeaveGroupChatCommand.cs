using MediatR;

namespace MessengerInfrastructure.Commands
{
    public class LeaveGroupChatCommand : IRequest
    {
        public Guid GroupChatId { get; set; }
        public string UserId { get; set; }

        public LeaveGroupChatCommand(Guid groupChatId, string userId)
        {
            GroupChatId = groupChatId;
            UserId = userId;
        }
    }
}
