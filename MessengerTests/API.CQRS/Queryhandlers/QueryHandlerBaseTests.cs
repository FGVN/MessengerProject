using System.Linq.Expressions;
using DataAccess;
using DataAccess.Models;
using MessengerInfrastructure.QueryHandlers;

namespace MessengerInfrastructure.QueryHandlers.Tests
{
    public class QueryHandlerBaseTests
    {
        [Fact]
        public void GetFilterProperties_ReturnsProperties()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new QueryHandlerMock(unitOfWorkMock.Object, null);

            // Act
            var result = handler.GetFilterProperties();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count()); // Assuming GetFilterProperties always returns two properties
            Assert.Contains("ChatId", result);
            Assert.Contains("UserId", result);
            Assert.Contains("ContactUserId", result);
        }

        [Fact]
        public void FilterEntities_ReturnsCorrectPredicate()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new QueryHandlerMock(unitOfWorkMock.Object, null);
            var properties = new List<string> { "ChatId", "UserId", "ContactUserId" };
            var query = "example";

            // Act
            var result = handler.FilterEntities(properties, query);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Func<UserChat, bool>>(result.Compile());
        }

        [Fact]
        public void PaginateEntities_ReturnsCorrectRange()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new QueryHandlerMock(unitOfWorkMock.Object, null);
            var entities = new List<UserChat>
            {
                new UserChat { UserId = "1" },
                new UserChat { UserId = "2" },
                new UserChat { UserId = "3" },
                new UserChat { UserId = "4" }
            };

            // Act
            var result = handler.PaginateEntities(entities, 1, 4).ToList();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("2", result[0].UserId);
            Assert.Equal("4", result[2].UserId);
        }
    }

    // This is a mock class inheriting from QueryHandlerBase to expose protected methods for testing
    public class QueryHandlerMock : QueryHandlerBase<UserChat, UserChatDTO>
    {
        public QueryHandlerMock(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(unitOfWork)
        {
        }

        public new IQueryable<UserChat> GetAllQueryable(Expression<Func<UserChat, bool>> predicate)
        {
            return base.GetAllQueryable(predicate);
        }

        public new IEnumerable<string> GetFilterProperties()
        {
            return base.GetFilterProperties();
        }

        public new Expression<Func<UserChat, bool>> FilterEntities(IEnumerable<string> properties, string query)
        {
            return base.FilterEntities(properties, query);
        }

        public new IQueryable<UserChat> SortEntities(IQueryable<UserChat> queryable, string sortBy, string sortDirection)
        {
            return base.SortEntities(queryable, sortBy, sortDirection);
        }

        public new IEnumerable<UserChat> PaginateEntities(IEnumerable<UserChat> entities, int from, int to)
        {
            return base.PaginateEntities(entities, from, to);
        }

        public override IEnumerable<string> GetFilterProperties(UserChat entity)
        {
            return new List<string> { "UserId", "ContactUserId" };
        }
    }
}
