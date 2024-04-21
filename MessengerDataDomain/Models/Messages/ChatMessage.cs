using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessengerDataAccess.Models.Chats;

namespace MessengerDataAccess.Models.Messages;
public class ChatMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public Guid ChatId { get; set; }
    [Required]
    [MaxLength(450)] 
    public string SenderId { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
    public bool IsGroupChat { get; set; }
    [ForeignKey("ChatId")] 
    public GroupChat GroupChat { get; set; }
}
