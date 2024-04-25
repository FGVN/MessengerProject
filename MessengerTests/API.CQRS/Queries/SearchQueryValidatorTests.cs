using DataAccess.Models;
using FluentValidation.TestHelper;

namespace MessengerInfrastructure.Query.Tests;

public class SearchQueryValidatorTests
{
    [Fact]
    public void ValidateSearchQuery_InvalidData_ShouldHaveErrors()
    {
        // Arrange
        var searchQuery = new SearchQuery<object>
        {
            Query = "",
            From = 0,
            To = 10,
            SortDirection = "", 
            SortBy = ""
        };

        var validator = new SearchQueryValidator<object>();

        // Act
        var result = validator.TestValidate(searchQuery);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Query);
        result.ShouldHaveValidationErrorFor(x => x.SortDirection);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void ValidateSearchQuery_ValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var searchQuery = new SearchQuery<User>
        {
            Query = "example",
            From = 5,
            To = 10,
            SortDirection = "desc",
            SortBy = "Id" 
        };

        var validator = new SearchQueryValidator<User>();

        // Act
        var result = validator.TestValidate(searchQuery);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Query);
        result.ShouldNotHaveValidationErrorFor(x => x.SortDirection);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

}
