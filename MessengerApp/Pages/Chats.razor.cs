using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public partial class ChatsPage : ComponentBase
{
    [Inject] private FindChatsQueryHandler queryHandler { get; set; }
    [Inject] private DeleteChatCommandHandler deleteChatHandler { get; set; }
    [Inject] private NavigationManager navigationManager { get; set; }
    [Inject] private CreateChatCommandHandler createChatHandler { get; set; }

    protected string SortBy { get; set; }
    protected string? SortDirection { get; set; }
    protected string PropertiesToGet { get; set; }
    protected IEnumerable<ChatMenuItem> chats;
    protected int pageNumber = 1;
    protected int totalChatsCount = 0;
    protected const int PageSize = 5;
    protected bool disableNext = false;
    protected bool disablePrevious = true;

    protected string selectedContactUsername;

    protected override async Task OnInitializedAsync()
    {
    }

    protected async Task HandleSearch()
    {
        pageNumber = 1;
        await LoadChats();
    }

    protected async Task LoadChats()
    {
        var findChatsQuery = new FindChatsQuery
        {
            // Query is empty for now, because userid will be used
            Query = "",
            SortBy = SortBy,
            SortDirection = SortDirection,
            From = (pageNumber - 1) * PageSize,
            To = pageNumber * PageSize,
            PropertiesToRetrieve = string.IsNullOrWhiteSpace(PropertiesToGet) ? null : PropertiesToGet.Split(',').Select(p => p.Trim())
        };
        var result = await queryHandler.Handle(findChatsQuery);
        chats = result;
        totalChatsCount = result.Count();
        StateHasChanged();

        UpdatePaginationState();
    }

    protected void NavigateToChat(Guid chatId)
    {
        navigationManager.NavigateTo($"/chats/{chatId}");
    }

    protected async Task DeleteChat(Guid chatId)
    {
        try
        {
            // Call the delete chat handler to delete the chat
            await deleteChatHandler.Handle(chatId);
            // Reload chats after deletion
            await LoadChats();
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error deleting chat: {ex.Message}");
        }
    }
    protected async Task CreateNewChat()
    {
        try
        {
            var chatId = await createChatHandler.Handle(selectedContactUsername);
            navigationManager.NavigateTo($"/chats/{chatId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating chat: {ex.Message}");
        }
    }

    protected async Task HandleNext()
    {
        pageNumber++;
        await LoadChats();
    }

    protected async Task HandlePrevious()
    {
        if (pageNumber > 1)
        {
            pageNumber--;
            await LoadChats();
        }
    }

    protected void UpdatePaginationState()
    {
        disablePrevious = pageNumber <= 1;
        disableNext = chats.Count() < PageSize;
    }
}