using MediatR;
using MessengerDataAccess.Models.Chats;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerInfrastructure.QueryHandlers
{
    public class MyGroupChatsQueryHandler : IRequestHandler<MyGroupChatsQuery, IEnumerable<GroupChat>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyGroupChatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GroupChat>> Handle(MyGroupChatsQuery request, CancellationToken cancellationToken)
        {
            var groupChatRepository = _unitOfWork.GetQueryRepository<GroupChatMembership>();

            var groupChats = await groupChatRepository
                .GetAllQueryable(m => m.UserId == request.UserId)
                .Select(m => m.GroupChat)
                .ToListAsync();

            return groupChats;
        }
    }
}
