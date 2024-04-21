using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public partial class EditGroupChatPage : ComponentBase
{

    [Inject] private UpdateGroupChatCommandHandler updateChatCommandHandler { get; set; }
    [Inject] private NavigationManager navigationManager { get; set; }
    [Parameter] public string ChatId { get; set; }
    protected string chatName;
    protected string chatDescription;
    protected async Task Handle()
    {
        try
        {
            var updateCommand = new UpdateGroupChatCommand
            {
                ChatId = Guid.Parse(ChatId),
                newName = chatName,
                newDescription = chatDescription
            };
            await updateChatCommandHandler.Handle(updateCommand);
            navigationManager.NavigateTo($"/chat/{ChatId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating chat: {ex.Message}");
        }
    }
}