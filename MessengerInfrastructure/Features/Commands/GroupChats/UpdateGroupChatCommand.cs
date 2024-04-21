using MediatR;

namespace MessengerInfrastructure.CommandHandlers
{
    public class UpdateGroupChatCommand : IRequest
    {
        public Guid ChatId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }
}
