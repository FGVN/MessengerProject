namespace MessengerInfrastructure.Services.DTOs
{
    public class CreateChatDTO
    {
        public Guid UserId { get; set; }
        public Guid ContactUserId { get; set; }
    }
}
