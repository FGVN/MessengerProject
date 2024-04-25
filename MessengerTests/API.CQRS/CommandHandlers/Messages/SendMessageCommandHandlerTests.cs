using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers.Tests;

public class SendMessageCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidMessage_ReturnsMessageId()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var senderId = "senderUserId";
        var message = "Test message";

        var command = new SendMessageCommand {SenderId = senderId, ChatId = chatId, Message = message };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var messageRepositoryMock = new Mock<IRepository<ChatMessage>>();

        messageRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<ChatMessage>()))
                             .Returns(Task.CompletedTask)
                             .Callback<ChatMessage>(msg =>
                             {
                                 msg.Id = 1; // Setting a mock ID for the newly created message
                             });

        unitOfWorkMock.Setup(uow => uow.GetRepository<ChatMessage>())
                      .Returns(messageRepositoryMock.Object);

        var handler = new SendMessageCommandHandler(unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result); // Expecting the mock ID set during the callback
        messageRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ChatMessage>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
