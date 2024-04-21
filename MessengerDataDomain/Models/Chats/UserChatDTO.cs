using MediatR;
using System.Text.Json.Serialization;

namespace MessengerDataAccess.Models.Chats;
public class UserChatDTO : IRequest<string>
{
    public Guid ChatId { get; set; }

    [JsonPropertyName("Username")]
    public string UserId { get; set; }
    [JsonPropertyName("ContactUsername")]
    public string ContactUserId { get; set; } 
}
