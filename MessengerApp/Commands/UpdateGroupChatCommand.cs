using System.ComponentModel.DataAnnotations;

public class UpdateGroupChatCommand
{
    public Guid ChatId { get; set; }

    public string newName { get; set; }

    public string newDescription { get; set; }
}
