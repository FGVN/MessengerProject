using DataDomain.Users;
using MediatR;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure.CommandHandlers;
using MessengerInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(AuthenticationSchemes = "Bearer")]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task JoinChatGroup(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(SendMessageDTO sendMessageDto)
    {
        try
        {
            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new SendMessageCommand(senderId, sendMessageDto);
            int messageId = await _mediator.Send(command);

            var userQuery = new GetUserByIdQuery(senderId);
            var senderUsername = (await _mediator.Send(userQuery)).Username;

            var chatMessage = new ChatMessage
            {
                Id = messageId,
                ChatId = sendMessageDto.ChatId,
                SenderId = senderUsername,
                Message = sendMessageDto.Message,
                Timestamp = DateTime.UtcNow
            };

            // Send message only to users in the chat group
            await Clients.Group(sendMessageDto.ChatId.ToString()).SendAsync("ReceiveMessage", chatMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public async Task EditMessage(int messageId, string newMessage, string chatId)
    {
        try
        {
            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new EditMessageCommand(senderId, messageId, newMessage);
            await _mediator.Send(command);

            // Send edited message only to users in the chat group
            await Clients.Group(chatId).SendAsync("MessageEdited", messageId, newMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }

    public async Task DeleteMessage(int messageId, string chatId)
    {
        try
        {
            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new DeleteMessageCommand(senderId, messageId);
            await _mediator.Send(command);

            // Send deleted message only to users in the chat group
            await Clients.Group(chatId).SendAsync("MessageDeleted", messageId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }

    public override async Task OnConnectedAsync()
    {
        var chatId = Context.GetHttpContext().Request.Query["chatId"];
        if (!string.IsNullOrEmpty(chatId))
        {
            Context.Items["ChatId"] = chatId;
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnConnectedAsync();
    }
}
