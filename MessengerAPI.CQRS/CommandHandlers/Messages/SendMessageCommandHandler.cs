using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();

        var chatMessage = new ChatMessage
        {
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Message = request.Message,
            Timestamp = DateTime.UtcNow
        };

        await chatMessageRepository.AddAsync(chatMessage);
        await _unitOfWork.SaveChangesAsync();

        // Return the ID of the newly created message
        return chatMessage.Id;
    }
}
