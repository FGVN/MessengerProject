using Microsoft.AspNetCore.Mvc;
using MessengerInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MessengerDataAccess.Models.Chats;
using MediatR;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat(string contactUsername)
    {
        string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Send a command to create a chat
        var chatId = await _mediator.Send(new CreateChatCommand(senderId, contactUsername));

        return Ok(new { ChatId = chatId });
    }

    [HttpDelete("deleteChat/{id}")]
    public async Task<IActionResult> DeleteChat(Guid id)
    {
        string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = await _mediator.Send(new DeleteChatCommand(senderId, id));

        if (success)
            return Ok();
        else
            return NotFound(); // or any appropriate response
    }

    [HttpGet("userchats")]
    public async Task<IEnumerable<UserChatDTO>> GetUserChats()
    {
        return await _mediator.Send(new GetAllUserChatsQuery());
    }

    [HttpPost("userchats/search")]
    public async Task<IEnumerable<object>> SearchUserChats(SearchQuery<UserChatDTO> query)
    {
        return await _mediator.Send(query);
    }
}
