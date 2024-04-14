using FluentValidation;
using System.Reflection;

public class SearchQueryValidator<T> : AbstractValidator<SearchQuery<T>>
{
    public SearchQueryValidator()
    {
        RuleFor(x => x.Query).NotEmpty().WithMessage("Query cannot be empty.");

        RuleFor(x => x.From).GreaterThanOrEqualTo(0).WithMessage("From must be greater than or equal to 0.");
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).WithMessage("To must be greater than or equal to From.");

        RuleFor(x => x.SortDirection).NotEmpty().WithMessage("SortDirection cannot be empty.");
        RuleFor(x => x.SortDirection).Matches("^(asc|desc)$").WithMessage("Sort direction must be 'asc' or 'desc'.");

        RuleFor(x => x.SortBy)
            .NotEmpty().WithMessage("SortBy cannot be empty.")
            .Must((query, sortBy) => ValidateSortBy(sortBy))
            .WithMessage($"SortBy must be a valid property of {typeof(T).Name}.");
    }

    private bool ValidateSortBy(string sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return false;

        // Use reflection to get all properties of T
        PropertyInfo[] properties = typeof(T).GetProperties();

        // Check if the sortBy value matches any property name in T
        foreach (var property in properties)
        {
            if (property.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
