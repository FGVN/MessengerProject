using DataAccess;
using MediatR;
using DataAccess.Models;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var userChatRepository = _unitOfWork.GetRepository<UserChat>();

        // Check if the user is a member of the chat
        var userChat = (await userChatRepository.GetAllAsync(uc => uc.UserId == request.UserId && uc.ChatId == request.ChatId)).FirstOrDefault();

        if (userChat == null)
        {
            // User is not a member of the chat
            return false;
        }

        // User is a member of the chat, proceed with deletion
        var chatRepository = _unitOfWork.GetRepository<UserChat>();

        // Delete the chat
        await chatRepository.DeleteAsync(userChat);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
