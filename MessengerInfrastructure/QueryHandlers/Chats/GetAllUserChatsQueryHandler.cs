using DataAccess;
using DataAccess.Models.Users;
using DataDomain.Users;
using MediatR;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Query;
using System.Linq.Expressions;

namespace MessengerInfrastructure.QueryHandlers;

public class GetAllUserChatsQueryHandler : IRequestHandler<GetAllUserChatsQuery, IEnumerable<UserChatDTO>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUserChatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserChatDTO>> Handle(GetAllUserChatsQuery request, CancellationToken cancellationToken)
    {
        var userChatRepository = _unitOfWork.GetRepository<UserChat>();
        var userChats = await userChatRepository.GetAllAsync(x => true);

        var userChatDTOs = new List<UserChatDTO>();
        foreach (var userChat in userChats)
        {
            var userDto = await GetUserDto(userChat.UserId);
            var contactUserDto = await GetUserDto(userChat.ContactUserId);

            var userChatDto = new UserChatDTO
            {
                ChatId = userChat.ChatId,
                UserId = userDto.Username,
                ContactUserId = contactUserDto.Username
            };

            userChatDTOs.Add(userChatDto);
        }

        return userChatDTOs;
    }

    private async Task<UserMenuItemDTO> GetUserDto(string userId)
    {
        Expression<Func<User, bool>> userFilterExpression = user => user.Id == userId;

        var userRepository = _unitOfWork.GetRepository<User>();

        var user = userRepository.GetAllQueryable(userFilterExpression).ToList().FirstOrDefault();

        if (user != null)
        {
            return new UserMenuItemDTO { Username = user.UserName, Email = user.Email };
        }

        return null;
    }
}
