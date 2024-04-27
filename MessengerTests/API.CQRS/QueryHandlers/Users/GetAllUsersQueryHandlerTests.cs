using System.Linq.Expressions;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Query;

namespace MessengerInfrastructure.QueryHandlers.Tests;

public class GetAllUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_Returns_Users()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IRepository<User>>();
        var users = new List<User>
        {
            new User { UserName = "User1", Email = "user1@example.com" },
            new User { UserName = "User2", Email = "user2@example.com" }
        };
        mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
              .ReturnsAsync(() => users.AsQueryable());
        mockUnitOfWork.Setup(uow => uow.GetRepository<User>()).Returns(mockUserRepository.Object);

        var handler = new GetAllUsersQueryHandler(mockUnitOfWork.Object);
        var request = new GetAllUsersQuery();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users.Count, result.Count());

        // Check if UserMenuItemDTO is correctly mapped
        foreach (var user in users)
        {
            Assert.Contains(result, u => u.Username == user.UserName && u.Email == user.Email);
        }
    }
}
