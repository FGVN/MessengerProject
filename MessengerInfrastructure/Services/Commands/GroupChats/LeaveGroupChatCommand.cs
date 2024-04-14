using MediatR;

namespace MessengerInfrastructure.CommandHandlers
{
    public class LeaveGroupChatCommand : IRequest
    {
        public Guid GroupChatId { get; set; }
        public string UserId { get; set; }
    }
}
