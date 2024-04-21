using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public partial class CreateChatPage : ComponentBase
{
    [Inject] private CreateGroupChatCommandHandler createGroupChatCommandHandler { get; set; }
    [Inject] JoinGroupChatCommandHandler joinGroupChatCommandHandler { get; set; }
    [Inject] NavigationManager navigationManager { get; set; }
    protected string chatName;
    protected string chatDescription;
    protected async Task HandleCreateChat()
    {
        try
        {
            var createCommand = new CreateGroupChatCommand
            {
                Name = chatName,
                Description = chatDescription
            };
            var chatId = await createGroupChatCommandHandler.Handle(createCommand);
            await joinGroupChatCommandHandler.Handle(chatId);

            navigationManager.NavigateTo($"/chats/{chatId}", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating chat: {ex.Message}");
        }
    }
}