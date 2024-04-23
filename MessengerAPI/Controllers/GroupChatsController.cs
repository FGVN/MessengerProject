using MediatR;
using MessengerInfrastructure.QueryHandlers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MessengerInfrastructure.Commands;
using DataAccess.Models;
using MessengerInfrastructure.Query;

namespace MessengerAPI.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]

public class GroupChatsController : Controller
{
    public readonly IMediator _mediator;

    public GroupChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat(CreateGroupChatCommand command)
    {
        var chatId = await _mediator.Send(command);
        return Ok(new { ChatId = chatId });
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinChat(string chatId)
    {
        await _mediator.Send(
            new JoinGroupChatCommand(
                Guid.Parse(chatId),
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            ));
        return Ok();
    }


    [HttpPost("update")]
    public async Task<IActionResult> UpdateChat(UpdateGroupChatCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("leave")]
    public async Task<IActionResult> LeaveChat(string chatId)
    {
        await _mediator.Send(
            new LeaveGroupChatCommand
            (
                Guid.Parse(chatId),
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            ));
        return Ok();
    }

    [HttpGet("myGroups")]
    public async Task<IEnumerable<GroupChat>> GetGroupChats()
    {
        return await _mediator.Send(new MyGroupChatsQuery { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
    }
}
