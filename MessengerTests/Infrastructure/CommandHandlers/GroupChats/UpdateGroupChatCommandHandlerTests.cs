using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class UpdateGroupChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_GroupChatExists_UpdatesNameAndDescription()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var newName = "New Group Chat Name";
        var newDescription = "New Group Chat Description";

        var existingGroupChat = new GroupChat { Id = chatId, Name = "Old Name", Description = "Old Description" };

        var groupChatRepositoryMock = new Mock<IRepository<GroupChat>>();
        groupChatRepositoryMock.Setup(repo => repo.GetByIdAsync(chatId.ToString()))
                               .ReturnsAsync(existingGroupChat);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChat>())
                      .Returns(groupChatRepositoryMock.Object);

        var handler = new UpdateGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new UpdateGroupChatCommand(chatId, newName, newDescription);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newName, existingGroupChat.Name);
        Assert.Equal(newDescription, existingGroupChat.Description);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GroupChatNotFound_ThrowsException()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var newName = "New Group Chat Name";
        var newDescription = "New Group Chat Description";

        var groupChatRepositoryMock = new Mock<IRepository<GroupChat>>();
        groupChatRepositoryMock.Setup(repo => repo.GetByIdAsync(chatId.ToString()))
                               .ReturnsAsync((GroupChat)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChat>())
                      .Returns(groupChatRepositoryMock.Object);

        var handler = new UpdateGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new UpdateGroupChatCommand(chatId, newName, newDescription);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
