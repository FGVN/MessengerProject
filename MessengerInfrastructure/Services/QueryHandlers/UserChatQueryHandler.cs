using DataAccess.Models.Users;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure;
using MessengerInfrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
    public class UserChatQueryHandler : QueryHandlerBase<UserChat, UserChatDTO>
    {
        private readonly UserQueryHandler _userQueryHandler;

        public UserChatQueryHandler(IUnitOfWork unitOfWork, UserQueryHandler userQueryHandler) : base(unitOfWork)
        {
            _userQueryHandler = userQueryHandler;
        }

        protected override IEnumerable<string> GetFilterProperties(UserChat entity)
        {
            return new List<string> { "UserId", "ContactUserId" };
        }

        public async Task<IEnumerable<UserChatDTO>> GetAllAsync()
        {
            var userChatRepository = _unitOfWork.GetQueryRepository<UserChat>();
            var userChats = await userChatRepository.GetAllAsync(x => true);

            var userChatDTOs = new List<UserChatDTO>();
            foreach (var userChat in userChats)
            {
                var userDto = await GetUserDto(userChat.UserId);
                var contactUserDto = await GetUserDto(userChat.ContactUserId);

                var userChatDto = new UserChatDTO
                {
                    ChatId = userChat.ChatId,
                    UserId = userDto?.Username,
                    ContactUserId = contactUserDto?.Username
                };

                userChatDTOs.Add(userChatDto);
            }

            return userChatDTOs;
        }

        private async Task<UserMenuItemDTO> GetUserDto(string userId)
        {
            // You need to implement this method to get the user DTO based on the user ID.
            return await _userQueryHandler.GetUserByIdAsync(userId);
        }

        public async Task<IEnumerable<object>> SearchAsync(SearchQuery<UserChatDTO> query)
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
