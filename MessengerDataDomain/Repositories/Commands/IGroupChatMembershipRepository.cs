using DataDomain.Repositories;
using MessengerDataAccess.Models.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDataAccess.Repositories.Commands
{
    interface IGroupChatMembershipCommandRepository : ICommandRepository<GroupChatMembership>
    {
    }
}
