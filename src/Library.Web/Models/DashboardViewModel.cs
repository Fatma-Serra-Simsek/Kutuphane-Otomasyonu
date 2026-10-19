using Library.Business.Dtos;

namespace Library.Web.Models;

public class DashboardViewModel
{
    public DashboardSummaryDto Summary { get; init; } = new();
    public IReadOnlyList<LoanListItemDto> RecentLoans { get; init; } = Array.Empty<LoanListItemDto>();
}
