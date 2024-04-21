using DataDomain.Repositories;
using MessengerDataAccess.Models.Chats;

namespace MessengerDataAccess.Repositories.Commands;
interface IGroupChatMembershipCommandRepository : ICommandRepository<GroupChatMembership>
{
}
