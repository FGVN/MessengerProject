using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataDomain.Users;
using DataAccess.Models.Users;
using MessengerInfrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
    {
        var tokenWithId = await _mediator.Send(registerUserDto);
        return string.IsNullOrEmpty(tokenWithId) ? BadRequest("User registration failed.") : Ok(new { Token = tokenWithId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginUserDto)
    {
        var tokenWithId = await _mediator.Send(loginUserDto);
        return string.IsNullOrEmpty(tokenWithId) ? Unauthorized("Invalid username or password.") : Ok(new { Token = tokenWithId });
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _mediator.Send(userId);
        return user == null ? NotFound("User not found.") : Ok(user);
    }

    [HttpGet("users")]
    public async Task<IEnumerable<UserMenuItemDTO>> GetUsers()
    {
        return await _mediator.Send(new GetAllUsersQuery());
    }

    [HttpPost("users/search")]
    public async Task<IEnumerable<object>> SearchUsers(SearchUsersQuery query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _mediator.Send(query);
    }
}
