using Microsoft.AspNetCore.Mvc;
using MessengerInfrastructure.Services;
using MessengerInfrastructure.Services.DTOs;
using System;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly CreateChatCommandHandler _createChatCommandHandler;
    private readonly SendMessageCommandHandler _sendMessageCommandHandler;

    public ChatController(CreateChatCommandHandler createChatCommandHandler, SendMessageCommandHandler sendMessageCommandHandler)
    {
        _createChatCommandHandler = createChatCommandHandler;
        _sendMessageCommandHandler = sendMessageCommandHandler;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat(CreateChatDTO createChatDto)
    {
        try
        {
            var chatId = await _createChatCommandHandler.Handle(createChatDto);
            return Ok(new { ChatId = chatId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the chat: {ex.Message}");
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(SendMessageDTO sendMessageDto)
    {
        try
        {
            await _sendMessageCommandHandler.Handle(sendMessageDto);
            return Ok();
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            return StatusCode(500, $"An error occurred while sending the message: {ex.Message}");
        }
    }
}
