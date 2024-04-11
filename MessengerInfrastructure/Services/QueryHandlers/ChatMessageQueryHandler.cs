using DataAccess.Models.Users;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure;
using MessengerInfrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
    public class ChatMessageQueryHandler : QueryHandlerBase<ChatMessage, ChatMessageDTO>
    {
        private readonly UserQueryHandler _userQueryHandler;

        public ChatMessageQueryHandler(IUnitOfWork unitOfWork, UserQueryHandler userQueryHandler) : base(unitOfWork)
        {
            _userQueryHandler = userQueryHandler;
        }

        protected override IEnumerable<string> GetFilterProperties(ChatMessage entity)
        {
            return new List<string> { "Id", "ChatId", "SenderId", "Message", "Timestamp" };
        }

        public async Task<IEnumerable<ChatMessageDTO>> GetAllAsync()
        {
            var chatMessageRepository = _unitOfWork.GetQueryRepository<ChatMessage>();
            var chatMessages = await chatMessageRepository.GetAllAsync(x => true);

            var chatMessageDTOs = new List<ChatMessageDTO>();
            foreach (var chatMessage in chatMessages)
            {
                var senderDto = await GetUserDto(chatMessage.SenderId);

                var chatMessageDto = new ChatMessageDTO
                {
                    Id = chatMessage.Id,
                    ChatId = chatMessage.ChatId,
                    SenderId = senderDto?.Username,
                    Message = chatMessage.Message,
                    Timestamp = chatMessage.Timestamp
                };

                chatMessageDTOs.Add(chatMessageDto);
            }

            return chatMessageDTOs;
        }

        private async Task<UserMenuItemDTO> GetUserDto(string userId)
        {
            return await _userQueryHandler.GetUserByIdAsync(userId);
        }

        public async Task<IEnumerable<object>> SearchAsync(SearchQuery<ChatMessageDTO> query)
        {
            var results = await base.SearchAsync(query);
            var found = new List<object>();
            foreach (var res in results)
            {
                var chatMessageDto = new ChatMessageDTO();
                var chatMessageDtoProperties = chatMessageDto.GetType().GetProperties();

                foreach (var prop in chatMessageDtoProperties)
                {
                    var propName = prop.Name;
                    var resProp = res.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (resProp != null)
                    {
                        var resValue = resProp.GetValue(res);
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
}
