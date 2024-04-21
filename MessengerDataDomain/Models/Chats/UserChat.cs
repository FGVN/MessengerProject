using System.ComponentModel.DataAnnotations;

namespace MessengerDataAccess.Models.Chats;
public class UserChat
{
    [Key]
    public Guid ChatId { get; set; }
    public string UserId { get; set; }
    public string ContactUserId { get; set; }
}
