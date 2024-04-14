using DataAccess.Models.Users;
using DataDomain.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{

    public class GetAllUsersQuery : IRequest<IEnumerable<UserMenuItemDTO>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserMenuItemDTO>>
    {

        public IUnitOfWork _unitOfWork { get; }
        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserMenuItemDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var users = await userRepository.GetAllAsync(x => true);
            return users.Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
        }
    }
}
