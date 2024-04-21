using MediatR;
using MessengerDataAccess.Models.Chats;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerInfrastructure.CommandHandlers
{

    public class UpdateGroupChatCommandHandler : IRequestHandler<UpdateGroupChatCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateGroupChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
        {
            var groupChatRepository = _unitOfWork.GetCommandRepository<GroupChat>();
            var groupChatQueryRepository = _unitOfWork.GetQueryRepository<GroupChat>();
            var groupChat = await groupChatQueryRepository.GetByIdAsync(request.ChatId.ToString());

            if (groupChat == null)
            {
                throw new Exception("Group chat not found.");
            }

            // Update the group chat's name and description
            groupChat.Name = request.NewName;
            groupChat.Description = request.NewDescription;

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
