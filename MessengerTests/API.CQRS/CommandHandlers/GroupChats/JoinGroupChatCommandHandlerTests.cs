using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class JoinGroupChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesMembership()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var membershipRepositoryMock = new Mock<IRepository<GroupChatMembership>>();

        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChatMembership>()).Returns(membershipRepositoryMock.Object);

        var handler = new JoinGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new JoinGroupChatCommand(Guid.Empty, "userId");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        membershipRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<GroupChatMembership>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
