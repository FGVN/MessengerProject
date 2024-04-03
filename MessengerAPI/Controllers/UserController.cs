using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessengerInfrastructure.Services;
using MessengerInfrastructure.Utilities;
using DataDomain.Users;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly UserManager<User> _userManager;
	private readonly JwtTokenGenerator _jwtTokenGenerator;

	public UsersController(UserManager<User> userManager, JwtTokenGenerator jwtTokenGenerator)
	{
		_userManager = userManager;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
	{
		try
		{
			var user = new User { UserName = registerUserDto.Username, Email = registerUserDto.Email };

			var result = await _userManager.CreateAsync(user, registerUserDto.Password);

			if (result.Succeeded)
			{

				var jwtToken = _jwtTokenGenerator.GenerateToken(registerUserDto.Username);

				return Ok(new { token = jwtToken });
			}
			else
			{
				return BadRequest(result.Errors);
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, "An error occurred while processing the request.");
		}
	}
}
