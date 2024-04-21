using DataAccess.Models.Users;
using MediatR;

namespace MessengerInfrastructure.Services
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserMenuItemDTO>>
    {
    }
}
