using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.QueryHandlers;
using System.Linq.Expressions;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class DeleteChatCommandHandlerTests
{
    private readonly Mock<IRepository<UserChat>> _userChatRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteChatCommandHandlerTests()
    {
        _userChatRepositoryMock = new Mock<IRepository<UserChat>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(uow => uow.GetRepository<UserChat>()).Returns(_userChatRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserIsMember_DeletesChat()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var chatId = Guid.NewGuid();
        var userChat = new UserChat { UserId = userId, ChatId = chatId };

        _userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                               .ReturnsAsync(new[] { userChat }.AsQueryable());

        var handler = new DeleteChatCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteChatCommand(userId, chatId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _userChatRepositoryMock.Verify(repo => repo.DeleteAsync(userChat), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserIsNotMember_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var chatId = Guid.NewGuid();

        _userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                               .ReturnsAsync(Enumerable.Empty<UserChat>().AsQueryable());

        var handler = new DeleteChatCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteChatCommand(userId, chatId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
