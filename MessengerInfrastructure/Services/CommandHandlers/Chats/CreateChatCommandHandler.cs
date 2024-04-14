using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataDomain.Repositories;
using DataDomain.Users;
using MediatR;
using MessengerDataAccess.Models.Chats;
using MessengerInfrastructure.Services.DTOs;

namespace MessengerInfrastructure.Services
{
    public class CreateChatCommand : IRequest<Guid>
    {
        public string SenderId { get; }
        public string ContactUsername { get; }

        public CreateChatCommand(string senderId, string contactUsername)
        {
            SenderId = senderId;
            ContactUsername = contactUsername;
        }
    }

    public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.GetQueryRepository<User>();
            var sender = (await userRepository.GetAllAsync(u => u.Id == request.SenderId)).FirstOrDefault();
            if (sender == null)
            {
                throw new Exception("Sender not found.");
            }

            var contactUser = (await userRepository.GetAllAsync(u => u.UserName == request.ContactUsername)).FirstOrDefault();
            if (contactUser == null)
            {
                throw new Exception("Contact user not found.");
            }

            var userChatRepository = _unitOfWork.GetCommandRepository<UserChat>();

            var userChat = new UserChat
            {
                UserId = sender.Id, // Use sender's user ID
                ContactUserId = contactUser.Id // Use contact user's ID
            };

            await userChatRepository.AddAsync(userChat);
            await _unitOfWork.SaveChangesAsync();

            return userChat.ChatId;
        }
    }
}
