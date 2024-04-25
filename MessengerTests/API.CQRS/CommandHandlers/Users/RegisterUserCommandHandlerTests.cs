using DataAccess.Models;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IJwtTokenGenerator> _tokenGeneratorMock;

    public RegisterUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        _tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
    }

    [Fact]
    public async Task Handle_ValidRegistration_ReturnsToken()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        var user = new User { UserName = username, Email = email };
        _userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success)
                        .Callback<User, string>((_, _) =>
                        {
                            // Setting the mock ID for the newly created user
                            user.Id = "1";
                        });

        _tokenGeneratorMock.Setup(tokenGen => tokenGen.GenerateToken(It.IsAny<string>()))
                           .Returns("mock_token");

        var handler = new RegisterUserCommandHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);
        var command = new RegisterUserCommand { Username = username, Email = email, Password = password };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("mock_token", result);
        _userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _tokenGeneratorMock.Verify(tokenGen => tokenGen.GenerateToken(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidUsername_ReturnsNull()
    {
        // Arrange
        var username = "invalid_username";
        var email = "test@example.com";
        var password = "password123";

        var registerUserCommand = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        _userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidUsername", Description = "Invalid username" }));

        var handler = new RegisterUserCommandHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await handler.Handle(registerUserCommand, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _tokenGeneratorMock.Verify(tokenGen => tokenGen.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsNull()
    {
        // Arrange
        var username = "testuser";
        var email = "invalid_email";
        var password = "password123";

        var registerUserCommand = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        _userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidEmail", Description = "Invalid email" }));

        var handler = new RegisterUserCommandHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await handler.Handle(registerUserCommand, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _tokenGeneratorMock.Verify(tokenGen => tokenGen.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WeakPassword_ReturnsNull()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "weak";

        var registerUserCommand = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        _userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "WeakPassword", Description = "Password is too weak" }));

        var handler = new RegisterUserCommandHandler(_userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await handler.Handle(registerUserCommand, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _tokenGeneratorMock.Verify(tokenGen => tokenGen.GenerateToken(It.IsAny<string>()), Times.Never);
    }
}
