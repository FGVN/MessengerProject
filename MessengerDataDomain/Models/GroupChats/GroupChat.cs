using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessengerDataAccess.Models.Chats;
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

    [JsonIgnore]
    public ICollection<GroupChatMembership> Members { get; set; }
}
