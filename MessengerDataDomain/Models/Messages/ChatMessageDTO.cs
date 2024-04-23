using MediatR;

namespace DataAccess.Models;

public class ChatMessageDTO : IRequest<string>
{
    public int Id { get; set; }
    public Guid ChatId { get; set; }
    public string SenderId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
