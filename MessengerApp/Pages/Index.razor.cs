using Microsoft.AspNetCore.Components;

namespace CodeBehind;

public partial class IndexPage : ComponentBase
{
    [Inject]
    protected FindUsersQueryHandler QueryHandler { get; set; }

    protected string Query { get; set; }
    protected string SortBy { get; set; }
    protected string? SortDirection { get; set; }
    protected string PropertiesToGet { get; set; }
    protected IEnumerable<UserMenuItem> Users;
    protected int pageNumber = 1;
    protected const int PageSize = 5;
    protected bool disableNext = false;
    protected bool disablePrevious = true;

    protected async Task HandleSearch()
    {
        pageNumber = 1;
        await LoadUsers();
    }

    protected async Task LoadUsers()
    {
        var findUsersQuery = new FindUsersQuery
        {
            Query = Query,
            SortBy = SortBy,
            SortDirection = SortDirection,
            From = (pageNumber - 1) * PageSize,
            To = pageNumber * PageSize,
            PropertiesToRetrieve = string.IsNullOrWhiteSpace(PropertiesToGet) ? null : PropertiesToGet.Split(',').Select(p => p.Trim())
        };
        var result = await QueryHandler.Handle(findUsersQuery);

        Users = result.Any() ? result : Enumerable.Empty<UserMenuItem>();

        UpdatePaginationState();
    }

    protected async Task HandleNext()
    {
        pageNumber++;
        await LoadUsers();
    }

    protected async Task HandlePrevious()
    {
        if (pageNumber > 1)
        {
            pageNumber--;
            await LoadUsers();
        }
    }

    protected void UpdatePaginationState()
    {
        disablePrevious = pageNumber <= 1;
        disableNext = Users.Count() < PageSize;
    }
}
