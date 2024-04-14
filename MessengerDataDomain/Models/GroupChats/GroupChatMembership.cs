using DataDomain.Users;
using System.ComponentModel.DataAnnotations;

namespace MessengerDataAccess.Models.Chats
{
    public class GroupChatMembership
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public string UserId { get; set; }

        // Navigation property to represent the group chat
        public GroupChat GroupChat { get; set; }

        // Navigation property to represent the user
        public User User { get; set; }
    }


}
