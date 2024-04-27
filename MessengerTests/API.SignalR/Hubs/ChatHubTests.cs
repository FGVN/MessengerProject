using DataAccess;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MessengerInfrastructure.Hubs.Tests;

public class ChatHubTests
{
    [Fact]
    public async Task JoinChatGroup_WhenCalled_AddsUserToGroup()
    {
        // Arrange
        var chatId = "testChatId";
        var context = new Mock<HubCallerContext>();
        var groups = new Mock<IGroupManager>();
        var hub = new ChatHub(Mock.Of<IMediator>(), Mock.Of<IUnitOfWork>())
        {
            Context = context.Object,
            Groups = groups.Object
        };

        // Act
        await hub.JoinChatGroup(chatId);

        // Assert
        groups.Verify(g => g.AddToGroupAsync(It.IsAny<string>(), chatId, default), Times.Once);
    }
}
