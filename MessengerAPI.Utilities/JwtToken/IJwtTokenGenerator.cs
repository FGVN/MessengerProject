namespace MessengerInfrastructure.Utilities;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userName);
}