﻿using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public partial class ChatsPage : ComponentBase
{
    [Inject] private FindChatsQueryHandler queryHandler { get; set; }
    [Inject] private DeleteChatCommandHandler deleteChatHandler { get; set; }
    [Inject] private NavigationManager navigationManager { get; set; }
    [Inject] private CreateChatCommandHandler createChatHandler { get; set; }
    [Inject] private FindUsersQueryHandler findUsersQueryHandler { get; set; }

    protected string Query { get; set; }
    protected string SortBy { get; set; }
    protected string SortDirection { get; set; }
    protected string PropertiesToGet { get; set; }
    protected IEnumerable<ChatMenuItem> chats;
    protected int PageNumber = 1;
    protected int totalChatsCount = 0;
    protected const int PageSize = 10;

    protected string selectedContactUsername;
    protected List<string> contactUsernames = new List<string>();

    protected override async Task OnInitializedAsync()
    {
        var res = await findUsersQueryHandler.Handle(new FindUsersQuery
        {
            Query = "a",
            From = 0,
            To = 100,
            SortBy = "username",
            SortDirection = "asc"
        }, 1);
        contactUsernames = res.Select(x => x.Username).ToList();
        StateHasChanged();
    }

    protected async Task HandleSearch()
    {
        PageNumber = 1;
        await LoadChats();
    }

    protected async Task LoadChats()
    {
        var findChatsQuery = new FindChatsQuery
        {
            Query = Query,
            SortBy = SortBy,
            SortDirection = SortDirection,
            From = (PageNumber - 1) * PageSize,
            To = PageNumber * PageSize,
            PropertiesToRetrieve = string.IsNullOrWhiteSpace(PropertiesToGet) ? null : PropertiesToGet.Split(',').Select(p => p.Trim())
        };
        var result = await queryHandler.Handle(findChatsQuery, PageNumber);
        chats = result;
        totalChatsCount = result.Count();
        StateHasChanged();
    }

    protected async Task HandlePageChange(int newPageIndex)
    {
        PageNumber = newPageIndex + 1;
        await LoadChats();
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
}