using MediatR;
using MessengerDataAccess.Models.Chats;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerInfrastructure.CommandHandlers
{
    public class LeaveGroupChatCommandHandler : IRequestHandler<LeaveGroupChatCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveGroupChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(LeaveGroupChatCommand request, CancellationToken cancellationToken)
        {
            var membershipRepository = _unitOfWork.GetQueryRepository<GroupChatMembership>();

            Expression<Func<GroupChatMembership, bool>> predicate =
                m => m.GroupId == request.GroupChatId && m.UserId == request.UserId;

            var membership = await membershipRepository.GetAllQueryable(predicate).FirstOrDefaultAsync();

            if (membership != null)
            {
                var commandRepository = _unitOfWork.GetCommandRepository<GroupChatMembership>();
                await commandRepository.DeleteAsync(membership);

                // Check if the chat has any members left
                var remainingMembersCount = await membershipRepository.GetAllQueryable(m => m.GroupId == request.GroupChatId).CountAsync();

                if (remainingMembersCount == 0)
                {
                    // If there are no remaining members, delete the chat
                    var chatRepository = _unitOfWork.GetCommandRepository<GroupChat>();
                    var chatToDelete = _unitOfWork.GetQueryRepository<GroupChat>().GetAllQueryable(m => m.Id == request.GroupChatId).FirstOrDefault();
                    if (chatToDelete != null)
                    {
                        await chatRepository.DeleteAsync(chatToDelete);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
