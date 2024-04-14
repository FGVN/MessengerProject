using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDataAccess.Models.Chats
{
    public class GroupChat
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public ICollection<GroupChatMembership> Members { get; set; }
    }


}
