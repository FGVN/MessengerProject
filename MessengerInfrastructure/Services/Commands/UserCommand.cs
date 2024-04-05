using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
	public class UserCommand : IUserCommand
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		private readonly UserManager<User> _userManager;

		public UserCommand(IUnitOfWork unitOfWork, IConfiguration configuration, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_userManager = userManager;
		}

		public async Task DeleteUserAsync(User user)
		{
			await _unitOfWork.GetCommandRepository<User>().DeleteAsync(user);
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task RegisterUserAsync(RegisterUserDTO registerUserDto)
		{
			// Hash the password before storing it
			var hashedPassword = HashPassword(registerUserDto.Password);

			var user = new User
			{
				UserName = registerUserDto.Username,
				Email = registerUserDto.Email,
				PasswordHash = hashedPassword  // Store the hashed password
			};

			await _unitOfWork.GetCommandRepository<User>().AddAsync(user);
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task<string> LoginUserAsync(LoginUserDTO loginUserDto)
		{
			// Find the user by email
			var user = await _userManager.FindByEmailAsync(loginUserDto.Email);

			// Verify the password
			if (user != null && await _userManager.CheckPasswordAsync(user, loginUserDto.Password))
			{
				// Generate JWT token
				var token = GenerateJwtToken(user);
				return token;
			}

			// Authentication failed
			return null;
		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				// Compute hash from the password bytes
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

				// Convert hashed bytes to hexadecimal string
				StringBuilder builder = new StringBuilder();
				foreach (byte b in hashedBytes)
				{
					builder.Append(b.ToString("x2"));
				}

				return builder.ToString();
			}
		}

		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.Email, user.Email)
                    // Add more claims as needed
                }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
