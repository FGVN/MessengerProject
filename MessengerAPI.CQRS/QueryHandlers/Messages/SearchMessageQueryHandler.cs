using DataAccess;
using DataAccess.Models;
using MediatR;
using System.Reflection;

namespace MessengerInfrastructure.QueryHandlers;

public class SearchMessageQueryHandler : QueryHandlerBase<ChatMessage, ChatMessageDTO>, IRequestHandler<SearchQuery<ChatMessageDTO>, IEnumerable<object>>
{

    public SearchMessageQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
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
                        var userDto = await _unitOfWork.GetRepository<User>().GetByIdAsync(resValue?.ToString());
                        prop.SetValue(chatMessageDto, userDto?.UserName);
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
