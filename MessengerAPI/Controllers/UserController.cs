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

	public UsersController(RegisterUserCommandHandler registerUserCommandHandler, LoginUserCommandHandler loginUserCommandHandler)
	{
		_registerUserCommandHandler = registerUserCommandHandler;
		_loginUserCommandHandler = loginUserCommandHandler;
	}
	//Mediator
	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
	{
		return Ok(await _registerUserCommandHandler.Handle(registerUserDto));
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginUserDTO loginUserDto)
	{
		return Ok(await _loginUserCommandHandler.Handle(loginUserDto));
	}
}
