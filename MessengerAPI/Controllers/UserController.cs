using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessengerInfrastructure.Services;
using DataDomain.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;
    private readonly UserQueryHandler _userQueryHandler;

    public UsersController(RegisterUserCommandHandler registerUserCommandHandler, LoginUserCommandHandler loginUserCommandHandler, UserQueryHandler userQueryHandler)
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
        _userQueryHandler = userQueryHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
    {
        var tokenWithId = await _registerUserCommandHandler.Handle(registerUserDto);
        if (string.IsNullOrEmpty(tokenWithId))
        {
            return BadRequest("User registration failed.");
        }
        else
        {
            return Ok(tokenWithId);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginUserDto)
    {
        var tokenWithId = await _loginUserCommandHandler.Handle(loginUserDto);
        if (string.IsNullOrEmpty(tokenWithId))
        {
            return Unauthorized("Invalid username or password.");
        }
        else
        {
            return Ok(tokenWithId);
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userQueryHandler.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        else
        {
            return Ok(user);
        }
    }
}
