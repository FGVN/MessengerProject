using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.QueryHandlers;
using System.Linq.Expressions;

namespace MessengerInfrastructure.Tests;

public class DeleteChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_UserIsMember_DeletesChat()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var chatId = Guid.NewGuid();
        var userChat = new UserChat { UserId = userId, ChatId = chatId };

        var userChatRepositoryMock = new Mock<IRepository<UserChat>>();
        userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                              .ReturnsAsync(new[] { userChat }.AsQueryable());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<UserChat>()).Returns(userChatRepositoryMock.Object);

        var handler = new DeleteChatCommandHandler(unitOfWorkMock.Object);
        var command = new DeleteChatCommand(userId, chatId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        userChatRepositoryMock.Verify(repo => repo.DeleteAsync(userChat), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserIsNotMember_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var chatId = Guid.NewGuid();

        var userChatRepositoryMock = new Mock<IRepository<UserChat>>();
        userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                              .ReturnsAsync(Enumerable.Empty<UserChat>().AsQueryable());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<UserChat>()).Returns(userChatRepositoryMock.Object);

        var handler = new DeleteChatCommandHandler(unitOfWorkMock.Object);
        var command = new DeleteChatCommand(userId, chatId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
