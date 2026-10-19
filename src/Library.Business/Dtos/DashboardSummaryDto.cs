namespace Library.Business.Dtos;

public class DashboardSummaryDto
{
    public int TotalBooks { get; init; }
    public int ActiveMembers { get; init; }
    public int ActiveLoans { get; init; }
    public int OverdueLoans { get; init; }
}
