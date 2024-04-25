using System.Security.Claims;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;

namespace MessengerAPI.Controllers.Tests;

public class ChatsControllerTests
{
    private readonly ChatsController _controller;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public ChatsControllerTests()
    {
        _controller = new ChatsController(_mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user_id"),
                }, "mock"))
            }
        };
    }

    [Fact]
    public async Task CreateDialog_Returns_ChatId()
    {
        // Arrange
        string contactUsername = "contact";
        var chatId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateChatCommand>(), CancellationToken.None)).ReturnsAsync(chatId);

        // Act
        var result = await _controller.CreateDialog(contactUsername);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var chatIdResult = Assert.IsType<Guid>(okResult.Value.GetType().GetProperty("ChatId").GetValue(okResult.Value));
        Assert.Equal(chatId, chatIdResult);
    }

    [Fact]
    public async Task DeleteChat_Returns_OkResult_If_Success()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteChatCommand>(), CancellationToken.None)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteChat(chatId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteChat_Returns_NotFound_If_Not_Success()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteChatCommand>(), CancellationToken.None)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteChat(chatId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUserChats_Returns_List_Of_UserChats()
    {
        // Arrange
        var userChats = new List<UserChatDTO>();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUserChatsQuery>(), CancellationToken.None)).ReturnsAsync(userChats);

        // Act
        var result = await _controller.GetUserChats();

        // Assert
        Assert.IsType<List<UserChatDTO>>(result);
    }

    [Fact]
    public async Task SearchUserChats_Returns_List_Of_Objects()
    {
        // Arrange
        var searchQuery = new SearchQuery<UserChatDTO>();
        var searchResult = new List<object>(); 
        _mediatorMock.Setup(m => m.Send(searchQuery, CancellationToken.None)).ReturnsAsync(searchResult);

        // Act
        var result = await _controller.SearchUserChats(searchQuery);

        // Assert
        Assert.IsType<List<object>>(result);
    }
}
