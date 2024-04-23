using DataAccess;
using MediatR;
using DataAccess.Models;
using MessengerInfrastructure.Commands;
using System.Linq.Expressions;

namespace MessengerInfrastructure.CommandHandlers;

public class LeaveGroupChatCommandHandler : IRequestHandler<LeaveGroupChatCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaveGroupChatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LeaveGroupChatCommand request, CancellationToken cancellationToken)
    {
        var membershipRepository = _unitOfWork.GetRepository<GroupChatMembership>();

        Expression<Func<GroupChatMembership, bool>> predicate =
            m => m.GroupId == request.GroupChatId && m.UserId == request.UserId;

        var membership = membershipRepository.GetAllQueryable(predicate).FirstOrDefault();

        if (membership != null)
        {
            var commandRepository = _unitOfWork.GetRepository<GroupChatMembership>();
            await commandRepository.DeleteAsync(membership);

            // Check if the chat has any members left
            var remainingMembersCount = membershipRepository.GetAllQueryable(m => m.GroupId == request.GroupChatId).Count();

            if (remainingMembersCount == 0)
            {
                // If there are no remaining members, delete the chat
                var chatRepository = _unitOfWork.GetRepository<GroupChat>();
                var chatToDelete = chatRepository.GetAllQueryable(m => m.Id == request.GroupChatId).FirstOrDefault();
                if (chatToDelete != null)
                {
                    await chatRepository.DeleteAsync(chatToDelete);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

}
