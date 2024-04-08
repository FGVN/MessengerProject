using FluentValidation;
using System.Reflection;

namespace DataAccess.Models.Users;
public class SearchUsersQuery
{
    public string? Query { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}

public class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.Query).NotEmpty().WithMessage("Query cannot be empty.");

        RuleFor(x => x.From).GreaterThanOrEqualTo(0).WithMessage("From must be greater than or equal to 0.");
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).WithMessage("To must be greater than or equal to From.");

        RuleFor(x => x.SortDirection).NotEmpty().WithMessage("SortDirection cannot be empty.");
        RuleFor(x => x.SortDirection).Matches("^(asc|desc)$").WithMessage("Sort direction must be 'asc' or 'desc'.");


        RuleFor(x => x.SortBy)
            .NotEmpty().WithMessage("SortBy cannot be empty.")
            .Must((query, sortBy) => ValidateSortBy(query, sortBy))
            .WithMessage("SortBy must be 'username' or 'email'.");
    }

    private bool ValidateSortBy(SearchUsersQuery query, string sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return false;

        // Use reflection to get all properties of UserMenuItemDTO
        PropertyInfo[] properties = typeof(UserMenuItemDTO).GetProperties();

        // Check if the sortBy value matches any property name in UserMenuItemDTO
        foreach (var property in properties)
        {
            if (property.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
