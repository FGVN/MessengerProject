using DataAccess;
using DataAccess.Models;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateChatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var userRepository = _unitOfWork.GetRepository<User>();
        var sender = (await userRepository.GetAllAsync(u => u.Id == request.SenderId)).FirstOrDefault();
        if (sender == null)
        {
            throw new NullReferenceException("Sender not found.");
        }

        var contactUser = (await userRepository.GetAllAsync(u => u.UserName == request.ContactUsername)).FirstOrDefault();
        if (contactUser == null)
        {
            throw new NullReferenceException("Contact user not found.");
        }

        var userChatRepository = _unitOfWork.GetRepository<UserChat>();

        var existingChat = await userChatRepository.GetAllAsync(
            uc => (uc.UserId == sender.Id && uc.ContactUserId == contactUser.Id) ||
                  (uc.UserId == contactUser.Id && uc.ContactUserId == sender.Id));

        if (existingChat.Any())
        {
            throw new Exception("A chat between these users already exists.");
        }
        else
        {
            var userChat = new UserChat
            {
                UserId = sender.Id,
                ContactUserId = contactUser.Id
            };

            await _unitOfWork.GetRepository<UserChat>().AddAsync(userChat);
            await _unitOfWork.SaveChangesAsync();

            return userChat.ChatId;
        }
    }

}
