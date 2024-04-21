using DataDomain.Users;
using System.ComponentModel.DataAnnotations;

namespace MessengerDataAccess.Models.Chats;
public class GroupChatMembership
{
    [Required]
    public Guid GroupId { get; set; }
    [Required]
    public string UserId { get; set; }
    public GroupChat GroupChat { get; set; }
    public User User { get; set; }
}
