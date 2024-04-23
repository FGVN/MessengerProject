using DataAccess.Models.Users;
using MediatR;

namespace MessengerInfrastructure.Query
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserMenuItemDTO>>
    {
    }
}
