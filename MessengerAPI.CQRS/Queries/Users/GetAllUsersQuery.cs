using DataAccess.Models;
using MediatR;

namespace MessengerInfrastructure.Query;

public class GetAllUsersQuery : IRequest<IEnumerable<UserMenuItemDTO>>
{
}
