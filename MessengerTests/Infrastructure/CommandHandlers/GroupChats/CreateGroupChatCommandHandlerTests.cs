using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class CreateGroupChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesGroupChat()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var groupChatRepositoryMock = new Mock<IRepository<GroupChat>>();

        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChat>()).Returns(groupChatRepositoryMock.Object);

        var handler = new CreateGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new CreateGroupChatCommand { Name = "Test Group", Description = "This is a test group." };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<Guid>(result);

        groupChatRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<GroupChat>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
