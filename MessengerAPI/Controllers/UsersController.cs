using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessengerInfrastructure.Services;
using DataDomain.Users;
using DataAccess.Models.Users;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;
    private readonly UserQueryHandler _userQueryHandler;

    public UsersController(RegisterUserCommandHandler registerUserCommandHandler, 
        LoginUserCommandHandler loginUserCommandHandler, UserQueryHandler userQueryHandler)
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
        _userQueryHandler = userQueryHandler;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
    {
        var tokenWithId = await _registerUserCommandHandler.Handle(registerUserDto);
        return string.IsNullOrEmpty(tokenWithId) ? BadRequest("User registration failed.") : Ok(new { Token = tokenWithId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginUserDto)
    {
        var tokenWithId = await _loginUserCommandHandler.Handle(loginUserDto);
        return string.IsNullOrEmpty(tokenWithId) ?  Unauthorized("Invalid username or password.") : Ok(new { Token = tokenWithId });
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userQueryHandler.GetUserByIdAsync(userId);
        return user == null ? NotFound("User not found.") : Ok(user);
    }



    [HttpGet("users")]
    public async Task<IEnumerable<UserMenuItemDTO>> GetUsers()
    {
        var userId = User.FindFirst("nameid")?.Value;

        return await _userQueryHandler.GetAllAsync();
    }

    [HttpPost("users/search")]
    public async Task<IEnumerable<object>> SearchUsers(SearchUsersQuery query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return await _userQueryHandler.SearchAsync(query);
    }
}
