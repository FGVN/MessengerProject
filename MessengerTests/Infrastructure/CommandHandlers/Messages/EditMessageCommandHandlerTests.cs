using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class EditMessageCommandHandlerTests
{
    [Fact]
    public async Task Handle_MessageExistsAndIsOwnedByUser_EditsMessage()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var newMessage = "New edited message";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId, Message = "Original message" };

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync(existingMessage);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new EditMessageCommandHandler(unitOfWorkMock.Object);
        var command = new EditMessageCommand(senderId, messageId, newMessage);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newMessage, existingMessage.Message);
        messageRepositoryMock.Verify(repo => repo.UpdateAsync(existingMessage), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_MessageNotFound_ThrowsException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var newMessage = "New edited message";

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync((ChatMessage)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new EditMessageCommandHandler(unitOfWorkMock.Object);
        var command = new EditMessageCommand(senderId, messageId, newMessage);

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
        var newMessage = "New edited message";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId, Message = "Original message" };

        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                             .ReturnsAsync(existingMessage);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new EditMessageCommandHandler(unitOfWorkMock.Object);
        var command = new EditMessageCommand(differentSenderId, messageId, newMessage);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        messageRepositoryMock.Verify(repo => repo.UpdateAsync(existingMessage), Times.Never);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
