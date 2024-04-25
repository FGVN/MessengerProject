using MediatR;
using DataAccess.Models;

namespace MessengerAPI.Controllers.Tests;

public class MessagesControllerTests
{
    private readonly MessagesController _controller;
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

    public MessagesControllerTests()
    {
        _controller = new MessagesController(_mediatorMock.Object);
    }

    [Fact]
    public async Task SearchChatMessages_Returns_List_Of_Objects()
    {
        // Arrange
        var searchQuery = new SearchQuery<ChatMessageDTO>();
        var searchResult = new List<object>();
        _mediatorMock.Setup(m => m.Send(searchQuery, CancellationToken.None)).ReturnsAsync(searchResult);

        // Act
        var result = await _controller.SearchChatMessages(searchQuery);

        // Assert
        Assert.IsType<List<object>>(result);
    }
}
