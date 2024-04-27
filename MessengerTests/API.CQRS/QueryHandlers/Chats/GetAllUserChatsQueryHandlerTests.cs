using System.Linq.Expressions;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Query;

namespace MessengerInfrastructure.QueryHandlers.Tests;

public class GetAllUserChatsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Returns_UserChatDTOs()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserChatRepository = new Mock<IRepository<UserChat>>();
        var mockUserRepository = new Mock<IRepository<User>>();

        var userChats = new List<UserChat>
        {
            new UserChat { ChatId = Guid.NewGuid(), UserId = "user1", ContactUserId = "user2" },
            new UserChat { ChatId = Guid.NewGuid(), UserId = "user2", ContactUserId = "user3" }
        };
        var users = new List<User>
        {
            new User { Id = "user1", UserName = "User1", Email = "user1@example.com" },
            new User { Id = "user2", UserName = "User2", Email = "user2@example.com" },
            new User { Id = "user3", UserName = "User3", Email = "user3@example.com" }
        };

        mockUserChatRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                      .ReturnsAsync(() => userChats.AsQueryable());

        mockUserRepository.Setup(repo => repo.GetAllQueryable(It.IsAny<Expression<Func<User, bool>>>()))
                          .Returns<Expression<Func<User, bool>>>(expression =>
                              users.Where(expression.Compile()).AsQueryable());

        mockUnitOfWork.Setup(uow => uow.GetRepository<UserChat>()).Returns(mockUserChatRepository.Object);
        mockUnitOfWork.Setup(uow => uow.GetRepository<User>()).Returns(mockUserRepository.Object);

        var handler = new GetAllUserChatsQueryHandler(mockUnitOfWork.Object);
        var request = new GetAllUserChatsQuery();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userChats.Count, result.Count());

        // Check if UserChatDTOs are correctly mapped
        foreach (var userChat in userChats)
        {
            var expectedUser = users.FirstOrDefault(u => u.Id == userChat.UserId);
            var expectedContactUser = users.FirstOrDefault(u => u.Id == userChat.ContactUserId);

            Assert.Contains(result, uc => uc.ChatId == userChat.ChatId &&
                                          uc.UserId == expectedUser?.UserName &&
                                          uc.ContactUserId == expectedContactUser?.UserName);
        }
    }
}
