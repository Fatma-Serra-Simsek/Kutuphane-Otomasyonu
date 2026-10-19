namespace Library.Domain.Common;

public class MemberBorrowStat
{
    public int MemberId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string MemberNumber { get; init; } = string.Empty;
    public int BorrowCount { get; init; }
}
