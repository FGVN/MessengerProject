using DataAccess.Models.Users;
using DataDomain.Users;
using MediatR;
using MessengerDataAccess.Models.Chats;
using System.Linq.Expressions;
using System.Reflection;

namespace MessengerInfrastructure.Services
{
    public class SearchChatQueryHandler : QueryHandlerBase<UserChat, UserChatDTO>, IRequestHandler<SearchQuery<UserChatDTO>, IEnumerable<object>>
    {
        public IUnitOfWork _unitOfWork;
        public SearchChatQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override IEnumerable<string> GetFilterProperties(UserChat entity)
        {
            return new List<string> { "UserId", "ContactUserId" };
        }
        private async Task<UserMenuItemDTO> GetUserDto(string userId)
        {
            Expression<Func<User, bool>> userFilterExpression = user => user.Id == userId;

            var userRepository = _unitOfWork.GetQueryRepository<User>();

            var user = userRepository.GetAllQueryable(userFilterExpression).ToList().FirstOrDefault();

            if (user != null)
            {
                return new UserMenuItemDTO { Username = user.UserName, Email = user.Email };
            }

            return null;
        }

        public async Task<IEnumerable<object>> Handle(SearchQuery<UserChatDTO> query, CancellationToken cancellationToken)
        {
            var results = await base.SearchAsync(query);
            var found = new List<object>();
            foreach (var res in results)
            {
                var userChatDto = new UserChatDTO();
                var userChatDtoProperties = userChatDto.GetType().GetProperties();

                foreach (var prop in userChatDtoProperties)
                {
                    var propName = prop.Name;
                    var resProp = res.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (resProp != null)
                    {
                        var resValue = resProp.GetValue(res);
                        if (propName.Equals("UserId", StringComparison.OrdinalIgnoreCase) || propName.Equals("ContactUserId", StringComparison.OrdinalIgnoreCase))
                        {
                            var userDto = await GetUserDto(resValue?.ToString());
                            prop.SetValue(userChatDto, userDto?.Username);
                        }
                        else
                        {
                            prop.SetValue(userChatDto, resValue);
                        }
                    }
                }
                found.Add(userChatDto);
            }



            return found;
        }
    }
}
