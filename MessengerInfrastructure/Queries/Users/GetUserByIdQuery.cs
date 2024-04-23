using DataAccess.Models.Users;
using MediatR;

namespace MessengerInfrastructure.Query;

public class GetUserByIdQuery : IRequest<UserMenuItemDTO>
{
    public string UserId { get; }

    public GetUserByIdQuery(string userId)
    {
        UserId = userId;
    }
}
