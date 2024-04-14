using MediatR;

namespace MessengerInfrastructure.Services
{
    public class CreateChatCommand : IRequest<Guid>
    {
        public string SenderId { get; }
        public string ContactUsername { get; }

        public CreateChatCommand(string senderId, string contactUsername)
        {
            SenderId = senderId;
            ContactUsername = contactUsername;
        }
    }
}
