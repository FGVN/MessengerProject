using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessengerInfrastructure.Services;
using MessengerInfrastructure.Utilities;
using DataDomain.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly RegisterUserCommandHandler _registerUserCommandHandler;
	private readonly JwtTokenGenerator _jwtTokenGenerator;

	public UsersController(RegisterUserCommandHandler registerUserCommandHandler, JwtTokenGenerator jwtTokenGenerator)
	{
		_registerUserCommandHandler = registerUserCommandHandler;
		_jwtTokenGenerator = jwtTokenGenerator; // Inject JwtTokenGenerator
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
	{
		try
		{
			await _registerUserCommandHandler.Handle(registerUserDto);


			var jwtToken = _jwtTokenGenerator.GenerateToken(registerUserDto.Username);

			return Ok(new { token = jwtToken });
		}
		catch (Exception ex)
		{
			return StatusCode(500, "An error occurred while processing the request.");
		}
	}
}
