using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessengerDataAccess.Models.Messages;

namespace MessengerAPI.Controllers;

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
}
