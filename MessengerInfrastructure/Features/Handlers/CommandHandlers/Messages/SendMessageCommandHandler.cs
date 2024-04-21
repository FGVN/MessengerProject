using MediatR;
using MessengerDataAccess.Models.Messages;
using MessengerInfrastructure;
using MessengerInfrastructure.CommandHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();

        var chatMessage = new ChatMessage
        {
            ChatId = request.MessageDto.ChatId,
            SenderId = request.SenderId,
            Message = request.MessageDto.Message,
            Timestamp = DateTime.UtcNow
        };

        await chatMessageRepository.AddAsync(chatMessage);
        await _unitOfWork.SaveChangesAsync();

        // Return the ID of the newly created message
        return chatMessage.Id;
    }
}
