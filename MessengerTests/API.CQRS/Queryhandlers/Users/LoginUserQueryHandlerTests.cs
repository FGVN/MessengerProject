using DataAccess.Models;
using MessengerInfrastructure.Query;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.QueryHandlers.Tests;

public class LoginUserQueryHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IJwtTokenGenerator> _tokenGeneratorMock;

    public LoginUserQueryHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        _tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
    }

    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var userId = "1";
        var token = "dummyToken";

        var loginUserDto = new LoginUserQuery { Email = email, Password = password };

        _userManagerMock.Setup(manager => manager.FindByEmailAsync(email))
                        .ReturnsAsync(new User { Id = userId, Email = email });
        _userManagerMock.Setup(manager => manager.CheckPasswordAsync(It.IsAny<User>(), password))
                        .ReturnsAsync(true);
        _tokenGeneratorMock.Setup(generator => generator.GenerateToken(userId))
                          .Returns(token);

        var handler = new LoginUserQueryHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await handler.Handle(loginUserDto, CancellationToken.None);

        // Assert
        Assert.Equal(token, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var loginUserDto = new LoginUserQuery { Email = email, Password = password };

        _userManagerMock.Setup(manager => manager.FindByEmailAsync(email))
                        .ReturnsAsync((User)null); // User not found

        var handler = new LoginUserQueryHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await handler.Handle(loginUserDto, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
