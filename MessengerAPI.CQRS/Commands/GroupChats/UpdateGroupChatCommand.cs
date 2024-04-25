using MediatR;

namespace MessengerInfrastructure.Commands
{
    public class UpdateGroupChatCommand : IRequest
    {
        public UpdateGroupChatCommand(Guid chatId, string newName, string newDescription)
        {
            ChatId = chatId;
            NewName = newName;
            NewDescription = newDescription;
        }

        public Guid ChatId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }
}
