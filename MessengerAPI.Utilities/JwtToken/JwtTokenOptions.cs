﻿namespace MessengerInfrastructure.Utilities; 

public class JwtTokenOptions
{
	public string Issuer { get; set; }
	public string Audience { get; set; }
	public string SecretKey { get; set; }
}
