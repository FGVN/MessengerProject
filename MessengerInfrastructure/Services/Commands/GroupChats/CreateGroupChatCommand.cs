using MediatR;

namespace MessengerInfrastructure.CommandHandlers
{
    public class CreateGroupChatCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
