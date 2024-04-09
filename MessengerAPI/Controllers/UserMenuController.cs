using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using MessengerInfrastructure.Services;
using DataAccess.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        var userId = User.FindFirst("nameid")?.Value;

        return await _userMenuQueryHandler.GetAllAsync();
    }

    [HttpPost("users/search")]
    public async Task<IEnumerable<object>> SearchUsers(SearchUsersQuery query)
    {
        var userId = User.FindFirst("nameid")?.Value;

        return await _userMenuQueryHandler.SearchAsync(query);
    }
}
