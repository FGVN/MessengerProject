using MediatR;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure.CommandHandlers;
using MessengerInfrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SendMessage(SendMessageDTO sendMessageDto, string jwtToken)
    {
        try
        {
            string senderId = ExtractSenderIdFromJwtToken(jwtToken);

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

            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public async Task EditMessage(int messageId, string newMessage, string jwtToken)
    {
        try
        {
            string senderId = ExtractSenderIdFromJwtToken(jwtToken);

            var command = new EditMessageCommand(senderId, messageId, newMessage);
            await _mediator.Send(command);

            await Clients.All.SendAsync("MessageEdited", messageId, newMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }

    public async Task DeleteMessage(int messageId, string jwtToken)
    {
        try
        {
            string senderId = ExtractSenderIdFromJwtToken(jwtToken);

            var command = new DeleteMessageCommand(senderId, messageId);
            await _mediator.Send(command);

            await Clients.All.SendAsync("MessageDeleted", messageId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }

    private string ExtractSenderIdFromJwtToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        return token.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
