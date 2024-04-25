using DataAccess;
using DataAccess.Models;

namespace MessengerInfrastructure.QueryHandlers.Tests;

public class SearchChatQueryHandlerTests
{
    [Fact]
    public void GetFilterProperties_ReturnsCorrectProperties()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new SearchChatQueryHandler(unitOfWorkMock.Object);
        var expectedProperties = new List<string> { "ChatId", "UserId", "ContactUserId" };

        // Act
        var result = handler.GetFilterProperties(new UserChat());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProperties, result);
    }
}
