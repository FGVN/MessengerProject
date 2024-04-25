using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public partial class GroupChatsPage : ComponentBase
{
    [Inject] MyGroupChatsQueryHandler groupChatsQueryHandler { get; set; }
    [Inject] LeaveGroupChatCommandHandler leaveGroupChatCommandHandler { get; set; }
    [Inject] NavigationManager navigationManager { get; set; }
    protected IEnumerable<GroupChat> groupChats;

    protected override async Task OnInitializedAsync()
    {
        await LoadGroupChats();
    }

    protected async Task LoadGroupChats()
    {
        try
        {
            groupChats = await groupChatsQueryHandler.Handle();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading group chats: {ex.Message}");
        }
    }

    protected async Task LeaveGroupChat(Guid chatId)
    {
        try
        {
            await leaveGroupChatCommandHandler.Handle(chatId);
            await LoadGroupChats();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leaving group chat: {ex.Message}");
        }
    }

    protected void NavigateToChat(Guid chatId)
    {
        navigationManager.NavigateTo($"/chats/{chatId}");
    }

    protected void NavigateToCreateChat()
    {
        navigationManager.NavigateTo("/createchat");
    }

    protected void NavigateToJoinChat()
    {
        navigationManager.NavigateTo("/joinchat");
    }

    protected void NavigateToEditChat(Guid chatId)
    {
        navigationManager.NavigateTo($"/editgroupchat/{chatId}");
    }
}