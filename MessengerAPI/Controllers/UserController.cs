using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MessengerInfrastructure.Services;
using DataDomain.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly RegisterUserCommandHandler _registerUserCommandHandler;

	public UsersController(RegisterUserCommandHandler registerUserCommandHandler)
	{
		_registerUserCommandHandler = registerUserCommandHandler;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
	{
		try
		{
			await _registerUserCommandHandler.Handle(registerUserDto);
			return Ok();
		}
		catch (Exception ex)
		{
			// Log the exception
			return StatusCode(500, "An error occurred while processing the request.");
		}
	}
}
