using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessengerInfrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using MessengerInfrastructure.CommandHandlers;
using MessengerDataAccess.Models.Messages;

namespace MessengerAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("chatmessages/search")]
        public async Task<IEnumerable<object>> SearchChatMessages(SearchQuery<ChatMessageDTO> query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(SendMessageDTO sendMessageDto)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new SendMessageCommand(senderId, sendMessageDto));
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteMessageCommand(senderId, id));
            return Ok();
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditMessage(EditMessageDTO editMessageDto)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new EditMessageCommand(senderId, editMessageDto.Id, editMessageDto.Message));
            return Ok();
        }
    }
}
