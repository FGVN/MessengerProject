using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MediatR;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class EditMessageCommandHandlerTests
{
    private readonly Mock<IRepository<ChatMessage>> _messageRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public EditMessageCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                       .Returns(_messageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_MessageExistsAndIsOwnedByUser_EditsMessage()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var newMessage = "New edited message";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId, Message = "Original message" };
        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync(existingMessage);

        var handler = new EditMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new EditMessageCommand(senderId, messageId, newMessage);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newMessage, existingMessage.Message);
        _messageRepositoryMock.Verify(repo => repo.UpdateAsync(existingMessage), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_MessageNotFound_ThrowsException()
    {
        // Arrange
        var messageId = 1;
        var senderId = "senderUserId";
        var newMessage = "New edited message";

        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync((ChatMessage)null);

        var handler = new EditMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new EditMessageCommand(senderId, messageId, newMessage);

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
        var newMessage = "New edited message";

        var existingMessage = new ChatMessage { Id = messageId, SenderId = senderId, Message = "Original message" };
        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId.ToString()))
                              .ReturnsAsync(existingMessage);

        var handler = new EditMessageCommandHandler(_unitOfWorkMock.Object);
        var command = new EditMessageCommand(differentSenderId, messageId, newMessage);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        _messageRepositoryMock.Verify(repo => repo.UpdateAsync(existingMessage), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
