using Microsoft.AspNetCore.Components;

namespace CodeBehind;
public class LogoutPage : ComponentBase
{
    [Inject] LogoutUserCommandHandler logoutHandler { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await logoutHandler.Handle();
    }
}
