using MediatR;

namespace MessengerInfrastructure.Commands;

public class SendMessageCommand : IRequest<int>
{
    public string SenderId { get; set; }
    public Guid ChatId { get; set; }
    public string Message { get; set; }
    public bool IsGroupChat { get; set; }
}
