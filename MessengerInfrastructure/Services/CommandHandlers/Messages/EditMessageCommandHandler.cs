using MediatR;
using MessengerInfrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using MessengerDataAccess.Models.Messages;

namespace MessengerInfrastructure.CommandHandlers
{
    public class EditMessageCommand : IRequest<Unit>
    {
        public string SenderId { get; }
        public int MessageId { get; }
        public string NewMessage { get; }

        public EditMessageCommand(string senderId, int messageId, string newMessage)
        {
            SenderId = senderId;
            MessageId = messageId;
            NewMessage = newMessage;
        }
    }

    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            var queryRepo = _unitOfWork.GetQueryRepository<ChatMessage>();
            var msg = await queryRepo.GetByIdAsync(request.MessageId.ToString());

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

            var chatMessageRepository = _unitOfWork.GetCommandRepository<ChatMessage>();
            await chatMessageRepository.UpdateAsync(msg);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
