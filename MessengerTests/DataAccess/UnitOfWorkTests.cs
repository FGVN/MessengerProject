using DataAccess.Models;
using DataAccess.Repositories;

namespace DataAccess.Tests;

public class UnitOfWorkTests
{
    [Fact]
    public void GetRepository_WhenRepositoryExists_ReturnsRepositoryInstance()
    {
        // Arrange
        var context = new Mock<MessengerDbContext>();
        var unitOfWork = new UnitOfWork(context.Object);

        // Act
        var userRepository = unitOfWork.GetRepository<User>();

        // Assert
        Assert.NotNull(userRepository);
        Assert.IsAssignableFrom<IRepository<User>>(userRepository);
    }

    [Fact]
    public void GetRepository_WhenRepositoryDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var context = new Mock<MessengerDbContext>();
        var unitOfWork = new UnitOfWork(context.Object);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => unitOfWork.GetRepository<NonExistentEntity>());
    }

    [Fact]
    public void InitializeRepositories_PopulatesRepositoriesDictionary()
    {
        // Arrange
        var context = new Mock<MessengerDbContext>();
        var unitOfWork = new UnitOfWork(context.Object);

        // Act
        unitOfWork.GetType().GetMethod("InitializeRepositories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(unitOfWork, null);

        // Assert
        Assert.NotEmpty(unitOfWork.GetType().GetField("_repositories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(unitOfWork) as Dictionary<Type, object>);
    }
    [Fact]
    public void Dispose_ProperlyDisposesUnitOfWork()
    {
        // Arrange
        var contextMock = new Mock<MessengerDbContext>();
        var unitOfWork = new UnitOfWork(contextMock.Object);

        // Act
        unitOfWork.Dispose();

        // Assert
        contextMock.Verify(c => c.Dispose(), Times.Once);
    }

    public class NonExistentEntity { }
}
