using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Query;

namespace MessengerInfrastructure.QueryHandlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserMenuItemDTO>>
{

    public IUnitOfWork _unitOfWork { get; }
    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserMenuItemDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var userRepository = _unitOfWork.GetRepository<User>();
        var users = await userRepository.GetAllAsync(x => true);
        return users.Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
    }
}
