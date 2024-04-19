using MediatR;
using MessengerDataAccess.Models.Chats;
using System;
using System.Collections.Generic;

namespace MessengerInfrastructure.QueryHandlers
{
    public class MyGroupChatsQuery : IRequest<IEnumerable<GroupChat>>
    {
        public string UserId { get; set; }
    }
}
