using MediatR;

namespace MessengerInfrastructure.Services.DTOs
{
    public class CreateChatDTO : IRequest<string>
    {
        public Guid UserId { get; set; }
        public Guid ContactUserId { get; set; }
    }
}
