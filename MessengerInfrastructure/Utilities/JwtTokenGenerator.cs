using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace MessengerInfrastructure.Utilities
{
	public class JwtTokenGenerator
	{
		private readonly JwtTokenOptions _jwtTokenOptions;

		public JwtTokenGenerator(IOptions<JwtTokenOptions> jwtTokenOptions)
		{
			_jwtTokenOptions = jwtTokenOptions.Value;
		}

		public string GenerateToken(string userName)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
					new Claim(ClaimTypes.NameIdentifier, userName),
					new Claim(ClaimTypes.Name, userName)
				}),
				Issuer = _jwtTokenOptions.Issuer,
				Audience = _jwtTokenOptions.Audience,
				SigningCredentials = creds
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
