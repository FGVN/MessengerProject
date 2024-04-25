using System.Security.Claims;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;

namespace MessengerAPI.Controllers.Tests;

public class GroupChatsControllerTests
{
    private readonly GroupChatsController _controller;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public GroupChatsControllerTests()
    {
        _controller = new GroupChatsController(_mediatorMock.Object);
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
    public async Task CreateChat_Returns_ChatId()
    {
        // Arrange
        var command = new CreateGroupChatCommand();
        var chatId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(chatId);

        // Act
        var result = await _controller.CreateChat(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var chatIdResult = Assert.IsType<Guid>(okResult.Value.GetType().GetProperty("ChatId").GetValue(okResult.Value));
        Assert.Equal(chatId, chatIdResult);
    }


    [Fact]
    public async Task JoinChat_Returns_OkResult()
    {
        // Arrange
        string chatId = Guid.NewGuid().ToString();
        var expectedCommand = new JoinGroupChatCommand(Guid.Parse(chatId), "user_id");

        // Act
        var result = await _controller.JoinChat(chatId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateChat_Returns_OkResult()
    {
        // Arrange
        var command = new UpdateGroupChatCommand(Guid.NewGuid(), "newName", "newDescription");

        // Act
        var result = await _controller.UpdateChat(command);

        // Assert
        Assert.IsType<OkResult>(result);
        _mediatorMock.Verify(m => m.Send(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task LeaveChat_Returns_OkResult()
    {
        // Arrange
        string chatId = Guid.NewGuid().ToString();
        var expectedCommand = new LeaveGroupChatCommand(Guid.Parse(chatId), "user_id");

        // Act
        var result = await _controller.LeaveChat(chatId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetGroupChats_Returns_List_Of_GroupChats()
    {
        // Arrange
        var groupChats = new List<GroupChat>(); 
        _mediatorMock.Setup(m => m.Send(It.IsAny<MyGroupChatsQuery>(), CancellationToken.None)).ReturnsAsync(groupChats);

        // Act
        var result = await _controller.GetGroupChats();

        // Assert
        Assert.IsType<List<GroupChat>>(result);
    }
}
