using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public class JoinChatPage : ComponentBase
{
    [Inject] JoinGroupChatCommandHandler joinGroupChatCommandHandler { get; set; }
    [Inject] NavigationManager navigationManager { get; set; }
    protected string chatId;
    protected async Task Handle()
    {
        try
        {
            await joinGroupChatCommandHandler.Handle(Guid.Parse(chatId));
            navigationManager.NavigateTo($"/chats/{chatId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error joining chat: {ex.Message}");
        }
    }
}
