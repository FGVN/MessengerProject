using DataAccess;
using DataAccess.Models;
using MediatR;

namespace MessengerInfrastructure.QueryHandlers;

public class SearchUsersQueryHandler : QueryHandlerBase<User, UserMenuItemDTO>, IRequestHandler<SearchQuery<UserMenuItemDTO>, IEnumerable<object>>
{
    public SearchUsersQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public Task<IEnumerable<object>> Handle(SearchQuery<UserMenuItemDTO> request, CancellationToken cancellationToken)
    {
        return base.SearchAsync(request);
    }
}
