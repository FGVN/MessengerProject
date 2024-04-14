using DataAccess.Models.Users;
using DataDomain.Users;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MessengerInfrastructure.Services
{
    public class SearchUsersQueryHandler : QueryHandlerBase<User, UserMenuItemDTO>, IRequestHandler<SearchUsersQuery, IEnumerable<object>>
    {
        public SearchUsersQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public Task<IEnumerable<object>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            return base.SearchAsync(request);
        }

        protected override IEnumerable<string> GetFilterProperties(User entity)
        {
            return new List<string> { entity.UserName, entity.Email };
        }
    }
}
