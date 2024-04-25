using Microsoft.Extensions.Options;

namespace MessengerInfrastructure.Utilities.Tests;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_ValidUserName_ReturnsValidToken()
    {
        // Arrange
        var userName = "testUser";
        var jwtTokenOptionsMock = new Mock<IOptions<JwtTokenOptions>>();
        jwtTokenOptionsMock.SetupGet(x => x.Value).Returns(new JwtTokenOptions
        {
            SecretKey = "faf8b04331c11fd65d864ebd0389410eaea4854741d5d805439ec34cca0699e0",
            Issuer = "testIssuer",
            Audience = "testAudience"
        });


        var tokenGenerator = new JwtTokenGenerator(jwtTokenOptionsMock.Object);

        // Act
        var token = tokenGenerator.GenerateToken(userName);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
    [Fact]
    public void GenerateToken_InvalidOptions_ReturnsEmptyString()
    {
        // Arrange
        var userName = "testUser";
        // Mocking invalid options
        var jwtTokenOptionsMock = new Mock<IOptions<JwtTokenOptions>>();
        jwtTokenOptionsMock.SetupGet(x => x.Value).Returns(new JwtTokenOptions
        {
            // Missing SecretKey, Issuer, and Audience intentionally
        });

        var tokenGenerator = new JwtTokenGenerator(jwtTokenOptionsMock.Object);

        // Act
        var token = tokenGenerator.GenerateToken(userName);

        // Assert
        Assert.Equal(string.Empty, token);
    }

    [Fact]
    public void GenerateToken_InvalidUserName_ReturnsEmptyString()
    {
        // Arrange
        // Mocking valid options
        var jwtTokenOptionsMock = new Mock<IOptions<JwtTokenOptions>>();
        jwtTokenOptionsMock.SetupGet(x => x.Value).Returns(new JwtTokenOptions
        {
            SecretKey = "testSecretKey",
            Issuer = "testIssuer",
            Audience = "testAudience"
        });

        var tokenGenerator = new JwtTokenGenerator(jwtTokenOptionsMock.Object);
        // Invalid user name with null value
        string userName = null;

        // Act
        var token = tokenGenerator.GenerateToken(userName);

        // Assert
        Assert.Equal(string.Empty, token);
    }
}
