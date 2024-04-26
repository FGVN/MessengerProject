using DataAccess;
using DataAccess.Models;
using MediatR;

namespace MessengerInfrastructure.QueryHandlers;

public class SearchGroupChatsQueryHandler : QueryHandlerBase<GroupChat, GroupChatDTO>, IRequestHandler<SearchQuery<GroupChatDTO>, IEnumerable<object>>
{
    public SearchGroupChatsQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public Task<IEnumerable<object>> Handle(SearchQuery<GroupChatDTO> request, CancellationToken cancellationToken)
    {
        return base.SearchAsync(request);
    }
}