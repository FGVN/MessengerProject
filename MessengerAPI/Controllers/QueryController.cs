using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessengerInfrastructure.Services;
using DataAccess.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Chats;
using MessengerDataAccess.Models.Messages;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //move
    public class QueryController : ControllerBase
    {
        private readonly UserQueryHandler _userMenuQueryHandler;
        private readonly ChatMessageQueryHandler _chatMessageQueryHandler;
        private readonly UserChatQueryHandler _userChatQueryHandler;

        public QueryController(UserQueryHandler userMenuQueryHandler, ChatMessageQueryHandler chatMessageQueryHandler, UserChatQueryHandler userChatQueryHandler)
        {
            _userMenuQueryHandler = userMenuQueryHandler;
            _chatMessageQueryHandler = chatMessageQueryHandler;
            _userChatQueryHandler = userChatQueryHandler;
        }

        [HttpGet("users")]
        public async Task<IEnumerable<UserMenuItemDTO>> GetUsers()
        {
            var userId = User.FindFirst("nameid")?.Value;

            return await _userMenuQueryHandler.GetAllAsync();
        }

        [HttpPost("users/search")]
        public async Task<IEnumerable<object>> SearchUsers(SearchUsersQuery query)
        {
            var userId = User.FindFirst("nameid")?.Value;

            return await _userMenuQueryHandler.SearchAsync(query);
        }

        [HttpGet("chatmessages")]
        public async Task<IEnumerable<ChatMessageDTO>> GetChatMessages()
        {
            return await _chatMessageQueryHandler.GetAllAsync();
        }

        [HttpGet("userchats")]
        public async Task<IEnumerable<UserChatDTO>> GetUserChats()
        {
            return await _userChatQueryHandler.GetAllAsync();
        }

        [HttpPost("chatmessages/search")]
        public async Task<IEnumerable<object>> SearchChatMessages(SearchQuery<ChatMessageDTO> query)
        {
            return await _chatMessageQueryHandler.SearchAsync(query);
        }

        [HttpPost("userchats/search")]
        public async Task<IEnumerable<object>> SearchUserChats(SearchQuery<UserChatDTO> query)
        {
            return await _userChatQueryHandler.SearchAsync(query);
        }
    }
}
