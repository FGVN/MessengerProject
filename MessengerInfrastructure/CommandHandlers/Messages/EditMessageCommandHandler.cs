using MediatR;
using DataAccess;
using MessengerInfrastructure.Commands;
using DataAccess.Models;

namespace MessengerInfrastructure.CommandHandlers;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();
        var msg = await chatMessageRepository.GetByIdAsync(request.MessageId.ToString());

        if (msg == null)
        {
            throw new ArgumentException($"Message with ID {request.MessageId} not found.");
        }

        // Check if the sender is the author of the message
        if (msg.SenderId != request.SenderId)
        {
            throw new UnauthorizedAccessException("You are not authorized to edit this message.");
        }

        // Update the message content
        msg.Message = request.NewMessage;

        await chatMessageRepository.UpdateAsync(msg);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
