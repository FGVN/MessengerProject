using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class DeleteMessageCommandHandlerTests
{
    [Fact]
    public async Task Handle_MessageExistsAndIsOwnedByUser_DeletesMessage()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId };

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync(existingMessage);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new DeleteMessageCommandHandler(unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(senderId, messageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        messageRepositoryMock.Verify(repo => repo.DeleteAsync(existingMessage), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_MessageNotFound_ThrowsException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync((ChatMessage)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new DeleteMessageCommandHandler(unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(senderId, messageId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_MessageOwnedByDifferentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var differentSenderId = "differentSenderId";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId };

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync(existingMessage);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new DeleteMessageCommandHandler(unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(differentSenderId, messageId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        messageRepositoryMock.Verify(repo => repo.DeleteAsync(existingMessage), Times.Never);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
