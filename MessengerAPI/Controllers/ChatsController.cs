using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using MessengerInfrastructure.Query;
using MessengerInfrastructure.Commands;
using DataAccess.Models;

namespace MessengerAPI.Controllers;

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
    public async Task<IActionResult> CreateDialog(string contactUsername)
    {
        var chatId = await _mediator.Send(new CreateChatCommand
        (
            User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(),
            contactUsername
        ));
        return Ok(new { ChatId = chatId });
    }
    [HttpDelete("deleteChat/{id}")]
    public async Task<IActionResult> DeleteChat(Guid id)
    {
        string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = await _mediator.Send(new DeleteChatCommand(senderId, id));
        return success ? Ok() : NotFound();
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
