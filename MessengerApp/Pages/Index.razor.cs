using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public class IndexPage : ComponentBase
{
    [Inject] FindUsersQueryHandler queryHandler { get; set; }
    protected string Query { get; set; }
    protected string SortBy { get; set; }
    protected string SortDirection { get; set; }
    protected string PropertiesToGet { get; set; }
    protected IEnumerable<UserMenuItem> users;
    protected int PageNumber = 1;
    protected int totalUsersCount;
    protected const int PageSize = 10; // Number of items per page

    protected async Task HandleSearch()
    {
        PageNumber = 1;
        await LoadUsers();
    }

    protected async Task LoadUsers()
    {
        var findUsersQuery = new FindUsersQuery
        {
            Query = Query,
            SortBy = SortBy,
            SortDirection = SortDirection,
            From = (PageNumber - 1) * PageSize,
            To = PageNumber * PageSize,
            PropertiesToRetrieve = string.IsNullOrWhiteSpace(PropertiesToGet) ? null : PropertiesToGet.Split(',').Select(p => p.Trim())
        };
        var result = await queryHandler.Handle(findUsersQuery, PageNumber);
        users = result; // Assuming the result is a paginated response with 'Items' property containing users
        totalUsersCount = result.Count();
        StateHasChanged();
    }

    protected async Task HandlePageChange(int newPageIndex)
    {
        PageNumber = newPageIndex + 1;
        await LoadUsers();
    }
}
