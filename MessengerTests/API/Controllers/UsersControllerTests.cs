using MessengerInfrastructure.Query;
using MessengerInfrastructure.Commands;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using MediatR;

namespace MessengerAPI.Controllers.Tests;

public class UsersControllerTests 
{
    private readonly UsersController _controller;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public UsersControllerTests()
    {
        _controller = new UsersController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Register_Returns_BadRequest_When_Registration_Fails()
    {
        // Arrange
        var command = new RegisterUserCommand();
        _mediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync("");

        // Act
        var result = await _controller.Register(command);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_Returns_Unauthorized_When_Login_Fails()
    {
        // Arrange
        var query = new LoginUserQuery();
        _mediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync("");

        // Act
        var result = await _controller.Login(query);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetUserById_Returns_NotFound_When_User_Not_Found()
    {
        // Arrange
        string userId = "some_id";
        _mediatorMock.Setup(m => m.Send(userId, CancellationToken.None)).ReturnsAsync(null);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetUsers_Returns_List_Of_Users()
    {
        // Arrange
        var userList = new List<UserMenuItemDTO>(); 
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), CancellationToken.None)).ReturnsAsync(userList);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        Assert.IsType<List<UserMenuItemDTO>>(result);
    }

    [Fact]
    public async Task SearchUsers_Returns_List_Of_Objects()
    {
        // Arrange
        var searchQuery = new SearchQuery<UserMenuItemDTO>();
        var searchResult = new List<object>(); 
        _mediatorMock.Setup(m => m.Send(searchQuery, CancellationToken.None)).ReturnsAsync(searchResult);

        // Act
        var result = await _controller.SearchUsers(searchQuery);

        // Assert
        Assert.IsType<List<object>>(result);
    }
}
