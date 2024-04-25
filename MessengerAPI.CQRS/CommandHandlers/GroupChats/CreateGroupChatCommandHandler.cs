using DataAccess;
using MediatR;
using DataAccess.Models;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class CreateGroupChatCommandHandler : IRequestHandler<CreateGroupChatCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroupChatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var groupChat = new GroupChat
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var groupChatRepository = _unitOfWork.GetRepository<GroupChat>();
        await groupChatRepository.AddAsync(groupChat);
        await _unitOfWork.SaveChangesAsync();

        return groupChat.Id;
    }
}
