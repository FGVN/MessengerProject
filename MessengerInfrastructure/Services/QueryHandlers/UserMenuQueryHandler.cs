using DataAccess.Models.Users;
using DataDomain.Users;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
    public class UserMenuQueryHandler : QueryHandlerBase<User, UserMenuItemDTO>
    {
        public UserMenuQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<IEnumerable<UserMenuItemDTO>> GetAllAsync()
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var users = await userRepository.GetAllAsync(x => true);
            return users.Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
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
