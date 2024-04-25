using DataAccess;
using DataAccess.Models;
using MediatR;
using System.Reflection;

namespace MessengerInfrastructure.QueryHandlers;
public class SearchChatQueryHandler : QueryHandlerBase<UserChat, UserChatDTO>, IRequestHandler<SearchQuery<UserChatDTO>, IEnumerable<object>>
{
    public SearchChatQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
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
                        var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(resValue?.ToString());
                        prop.SetValue(userChatDto, user?.UserName);
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
