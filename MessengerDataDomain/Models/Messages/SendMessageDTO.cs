namespace MessengerDataAccess.Models.Messages;
public class SendMessageDTO
{
    public Guid ChatId { get; set; }
    public string Message { get; set; }
    public bool IsGroupChat { get; set; }
}
