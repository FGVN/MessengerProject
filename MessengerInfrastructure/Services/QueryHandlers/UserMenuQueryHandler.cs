using DataAccess.Models.Users;
using DataDomain.Users;

namespace MessengerInfrastructure.Services
{
    public class UserQueryHandler : QueryHandlerBase<User, UserMenuItemDTO>
    {
        public UserQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<IEnumerable<UserMenuItemDTO>> GetAllAsync()
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var users = await userRepository.GetAllAsync(x => true);
            return users.Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
        }

        public async Task<UserMenuItemDTO> GetUserByIdAsync(string userId)
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            return new UserMenuItemDTO { Username = user.UserName, Email = user.Email };
        }

        public Task<IEnumerable<object>> SearchAsync(SearchQuery<UserMenuItemDTO> query)
        {
            return base.SearchAsync(query);
        }

        protected override IEnumerable<string> GetFilterProperties(User entity)
        {
            return new List<string> { entity.UserName, entity.Email };
        }
    }
}
