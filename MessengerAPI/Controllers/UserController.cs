using DataDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly UserManager<User> _userManager;

	public UsersController(UserManager<User> userManager)
	{
		_userManager = userManager;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
	{
		// Create new User and store in Identity database
		var user = new User
		{
			Username = registerUserDto.Username,
			Email = registerUserDto.Email
		};

		var result = await _userManager.CreateAsync(user, registerUserDto.Password);
		if (result.Succeeded)
		{
			return Ok();
		}
		else
		{
			// Registration failed, return error messages
			return BadRequest(result.Errors);
		}
	}
}
