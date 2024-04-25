using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class DeleteMessageCommandHandlerTests
{
    private readonly Mock<IRepository<ChatMessage>> _messageRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteMessageCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                       .Returns(_messageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_MessageExistsAndIsOwnedByUser_DeletesMessage()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId };
        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync(existingMessage);

        var handler = new DeleteMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(senderId, messageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _messageRepositoryMock.Verify(repo => repo.DeleteAsync(existingMessage), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_MessageNotFound_ThrowsException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";

        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync((ChatMessage)null);

        var handler = new DeleteMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(senderId, messageId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_MessageOwnedByDifferentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var differentSenderId = "differentSenderId";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId };
        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync(existingMessage);

        var handler = new DeleteMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteMessageCommand(differentSenderId, messageId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        _messageRepositoryMock.Verify(repo => repo.DeleteAsync(existingMessage), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
