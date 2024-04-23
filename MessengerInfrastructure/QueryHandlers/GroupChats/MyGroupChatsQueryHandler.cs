using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Query;
using Microsoft.EntityFrameworkCore;

namespace MessengerInfrastructure.QueryHandlers;

public class MyGroupChatsQueryHandler : IRequestHandler<MyGroupChatsQuery, IEnumerable<GroupChat>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MyGroupChatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<GroupChat>> Handle(MyGroupChatsQuery request, CancellationToken cancellationToken)
    {
        var groupChatRepository = _unitOfWork.GetRepository<GroupChatMembership>();

        var groupChats = await groupChatRepository
            .GetAllQueryable(m => m.UserId == request.UserId)
            .Select(m => m.GroupChat)
            .ToListAsync();

        return groupChats;
    }
}
