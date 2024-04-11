using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDataAccess.Models.Messages
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public Guid ChatId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
