using DataAccess;
using DataAccess.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace MessengerInfrastructure.QueryHandlers;

public class SearchMessageQueryHandler : QueryHandlerBase<ChatMessage, ChatMessageDTO>, IRequestHandler<SearchQuery<ChatMessageDTO>, IEnumerable<object>>
{

    public SearchMessageQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    protected override IEnumerable<string> GetFilterProperties(ChatMessage entity)
    {
        return new List<string> { "Id", "ChatId", "SenderId", "Message", "Timestamp" };
    }
    private async Task<UserMenuItemDTO> GetUserDto(string userId)
    {
        Expression<Func<User, bool>> userFilterExpression = user => user.Id == userId;

        var userRepository = _unitOfWork.GetRepository<User>();

        var users = await userRepository.GetAllQueryable(userFilterExpression).ToListAsync();
        var user = users.FirstOrDefault();

        if (user != null)
        {
            return new UserMenuItemDTO { Username = user.UserName, Email = user.Email };
        }

        return new UserMenuItemDTO();
    }
    public async Task<IEnumerable<object>> Handle(SearchQuery<ChatMessageDTO> query, CancellationToken cancellationToken)
    {
        var results = await base.SearchAsync(query);
        var found = new List<object>();
        foreach (var item in results)
        {
            if (item == null)
                continue;
            var chatMessageDto = new ChatMessageDTO();
            var chatMessageDtoProperties = chatMessageDto.GetType().GetProperties();

            foreach (var prop in chatMessageDtoProperties)
            {
                var propName = prop.Name;
                var resProp = item.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (resProp != null)
                {
                    var resValue = resProp.GetValue(item);
                    if (propName.Equals("SenderId", StringComparison.OrdinalIgnoreCase))
                    {
                        var userDto = await GetUserDto(resValue?.ToString());
                        prop.SetValue(chatMessageDto, userDto?.Username);
                    }
                    else
                    {
                        prop.SetValue(chatMessageDto, resValue);
                    }
                }
            }
            found.Add(chatMessageDto);
        }
        return found;
    }
}
