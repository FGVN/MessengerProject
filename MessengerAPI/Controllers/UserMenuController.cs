using Microsoft.AspNetCore.Mvc;
using MessengerInfrastructure.Services.QueryHandlers;
using DataAccess.Models.Users;

[ApiController]
[Route("api/[controller]")]
public class UserMenuController : ControllerBase
{
    private readonly UserMenuQueryHandler _userMenuQueryHandler;

    public UserMenuController(UserMenuQueryHandler userMenuQueryHandler)
    {
        _userMenuQueryHandler = userMenuQueryHandler;
    }

    [HttpGet("users")]
    public async Task<IEnumerable<UserMenuItemDTO>> GetUsers()
    {
        return await _userMenuQueryHandler.GetUsersAsync();
    }

    [HttpPost("users/search")]
    public async Task<IEnumerable<UserMenuItemDTO>> SearchUsers(SearchUsersQuery query)
    {
        return await _userMenuQueryHandler.SearchUsersAsync(query);
    }
}
