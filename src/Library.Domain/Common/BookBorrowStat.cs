namespace Library.Domain.Common;

public class BookBorrowStat
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int BorrowCount { get; init; }
}
