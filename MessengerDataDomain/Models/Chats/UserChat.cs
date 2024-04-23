using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class UserChat
{
    [Key]
    public Guid ChatId { get; set; }
    public string UserId { get; set; }
    public string ContactUserId { get; set; }
}
