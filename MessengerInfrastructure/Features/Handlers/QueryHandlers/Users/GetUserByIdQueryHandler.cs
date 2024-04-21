using DataAccess.Models.Users;
using DataDomain.Users;
using MediatR;
using MessengerInfrastructure.Services;

namespace MessengerInfrastructure.QueryHandlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserMenuItemDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserMenuItemDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var user = await userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                return null;
            }

            return new UserMenuItemDTO { Username = user.UserName, Email = user.Email };
        }
    }

}
