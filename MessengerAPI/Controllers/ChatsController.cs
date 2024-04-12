using Microsoft.AspNetCore.Mvc;
using MessengerInfrastructure.Services;
using MessengerInfrastructure.Services.DTOs;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MessengerInfrastructure.CommandHandlers;
using MessengerDataAccess.Models.Chats;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly CreateChatCommandHandler _createChatCommandHandler;
    private readonly SendMessageCommandHandler _sendMessageCommandHandler;
    private readonly DeleteMessageCommandHandler _deleteMessageCommandHandler;
    private readonly EditMessageCommandHandler _editMessageCommandHandler;
    private readonly DeleteChatCommandHandler _deleteChatCommandHandler;
    private readonly UserChatQueryHandler _userChatQueryHandler;

    public ChatsController(CreateChatCommandHandler createChatCommandHandler,
                           SendMessageCommandHandler sendMessageCommandHandler,
                           DeleteMessageCommandHandler deleteMessageCommandHandler,
                           EditMessageCommandHandler editMessageCommandHandler,
                           DeleteChatCommandHandler deleteChatCommandHandler,
                           UserChatQueryHandler userChatQueryHandler)
    {
        _createChatCommandHandler = createChatCommandHandler;
        _sendMessageCommandHandler = sendMessageCommandHandler;
        _deleteMessageCommandHandler = deleteMessageCommandHandler;
        _editMessageCommandHandler = editMessageCommandHandler;
        _deleteChatCommandHandler = deleteChatCommandHandler;
        _userChatQueryHandler = userChatQueryHandler;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat(string contactUsername)
    {
        string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var chatId = await _createChatCommandHandler.Handle(senderId, contactUsername);
        return Ok(new { ChatId = chatId });
    }

    [HttpDelete("deleteChat/{id}")]
    public async Task<IActionResult> DeleteChat(Guid id)
    {
        string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _deleteChatCommandHandler.Handle(senderId, id);
        return Ok();
    }


    [HttpGet("userchats")]
    public async Task<IEnumerable<UserChatDTO>> GetUserChats()
    {
        return await _userChatQueryHandler.GetAllAsync();
    }

    [HttpPost("userchats/search")]
    public async Task<IEnumerable<object>> SearchUserChats(SearchQuery<UserChatDTO> query)
    {
        return await _userChatQueryHandler.SearchAsync(query);
    }
}
