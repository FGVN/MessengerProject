using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;
using System.Linq.Expressions;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class LeaveGroupChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_UserIsMember_RemovesFromMembership()
    {
        // Arrange
        var groupId = Guid.Empty;
        var userId = "userId";

        var membership = new GroupChatMembership { GroupId = groupId, UserId = userId };
        var memberships = new List<GroupChatMembership> { membership };

        var membershipRepositoryMock = new Mock<IRepository<GroupChatMembership>>();
        membershipRepositoryMock.Setup(repo => repo.GetAllQueryable(It.IsAny<Expression<Func<GroupChatMembership, bool>>>()))
                        .Returns(memberships.AsQueryable());

        var chatRepositoryMock = new Mock<IRepository<GroupChat>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChatMembership>()).Returns(membershipRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChat>()).Returns(chatRepositoryMock.Object);

        var handler = new LeaveGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new LeaveGroupChatCommand(groupId, userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        membershipRepositoryMock.Verify(repo => repo.DeleteAsync(membership), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserIsLastMember_DeletesGroupChat()
    {
        // Arrange
        var groupId = Guid.Empty;
        var userId = "userId";

        var membership = new GroupChatMembership { GroupId = groupId, UserId = userId };
        var memberships = new List<GroupChatMembership> { membership };

        var membershipRepositoryMock = new Mock<IRepository<GroupChatMembership>>();
        membershipRepositoryMock.Setup(repo => repo.GetAllQueryable(It.IsAny<Expression<Func<GroupChatMembership, bool>>>()))
                                .Returns(memberships.AsQueryable());
        membershipRepositoryMock.Setup(repo => repo.GetAllQueryable(m => m.GroupId == groupId)).Returns(memberships.AsQueryable());

        var chatRepositoryMock = new Mock<IRepository<GroupChat>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChatMembership>()).Returns(membershipRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<GroupChat>()).Returns(chatRepositoryMock.Object);

        var handler = new LeaveGroupChatCommandHandler(unitOfWorkMock.Object);
        var command = new LeaveGroupChatCommand(groupId, userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        membershipRepositoryMock.Verify(repo => repo.DeleteAsync(membership), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
