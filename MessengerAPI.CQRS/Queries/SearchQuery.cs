using MediatR;

public class SearchQuery<T> : IRequest<IEnumerable<object>>
{
    public string? Query { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public IEnumerable<string>? PropertiesToRetrieve { get; set; } = null;
}
