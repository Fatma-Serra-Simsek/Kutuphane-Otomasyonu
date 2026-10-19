namespace Library.Domain.Common;

public class SearchQuery
{
    public const int DefaultPageSize = 20;

    public string? Term { get; init; }
    public int? CategoryId { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;
}
