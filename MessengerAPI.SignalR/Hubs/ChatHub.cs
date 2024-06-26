﻿using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MessengerInfrastructure.Hubs;

[Authorize(AuthenticationSchemes = "Bearer")]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task JoinChatGroup(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(SendMessageCommand command)
    {
        try
        {
            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            command.SenderId = senderId;

            int messageId = await _mediator.Send(command);

            var user = _unitOfWork.GetRepository<User>().GetAllQueryable(x => x.Id == senderId).AsQueryable().FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }

            var senderUsername = user.UserName;
            var chatMessage = new ChatMessage
            {
                Id = messageId,
                ChatId = command.ChatId,
                SenderId = senderUsername,
                Message = command.Message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(command.ChatId.ToString()).SendAsync("ReceiveMessage", chatMessage);
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
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await Clients.Client(Context.ConnectionId).SendAsync("CloseConnection");
    }
}
