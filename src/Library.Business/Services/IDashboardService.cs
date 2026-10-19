using Library.Business.Dtos;

namespace Library.Business.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanListItemDto>> GetRecentLoansAsync(int count, CancellationToken cancellationToken = default);
}
