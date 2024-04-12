using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessengerInfrastructure.Services;
using DataAccess.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Chats;
using MessengerDataAccess.Models.Messages;
using System.Security.Claims;
using MessengerInfrastructure.CommandHandlers;

namespace MessengerAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    //move
    public class MessagesController : ControllerBase
    {
        private readonly ChatMessageQueryHandler _chatMessageQueryHandler;
        private readonly SendMessageCommandHandler _sendMessageCommandHandler;
        private readonly EditMessageCommandHandler _editMessageCommandHandler;
        private readonly DeleteMessageCommandHandler _deleteMessageCommandHandler;

        public MessagesController(ChatMessageQueryHandler chatMessageQueryHandler, SendMessageCommandHandler sendMessageCommandHandler,
            EditMessageCommandHandler editMessageCommandHandler, DeleteMessageCommandHandler deleteMessageCommandHandler)

        {
            _chatMessageQueryHandler = chatMessageQueryHandler;
            _sendMessageCommandHandler = sendMessageCommandHandler;
            _editMessageCommandHandler = editMessageCommandHandler;
            _deleteMessageCommandHandler = deleteMessageCommandHandler;
        }

        [HttpGet("chatmessages")]
        public async Task<IEnumerable<ChatMessageDTO>> GetChatMessages()
        {
            return await _chatMessageQueryHandler.GetAllAsync();
        }

        [HttpPost("chatmessages/search")]
        public async Task<IEnumerable<object>> SearchChatMessages(SearchQuery<ChatMessageDTO> query)
        {
            return await _chatMessageQueryHandler.SearchAsync(query);
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(SendMessageDTO sendMessageDto)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _sendMessageCommandHandler.Handle(senderId, sendMessageDto);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _deleteMessageCommandHandler.Handle(senderId, id);
            return Ok();
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditMessage(EditMessageDTO editMessageDto)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _editMessageCommandHandler.Handle(senderId, editMessageDto);
            return Ok();
        }
    }
}
