using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var queryRepo = _unitOfWork.GetRepository<ChatMessage>();
        var msg = await queryRepo.GetByIdAsync(request.MessageId.ToString());

        if (msg == null)
        {
            throw new ArgumentException($"Message with ID {request.MessageId} not found.");
        }

        if (msg.SenderId != request.SenderId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this message.");
        }

        var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();
        await chatMessageRepository.DeleteAsync(msg);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
