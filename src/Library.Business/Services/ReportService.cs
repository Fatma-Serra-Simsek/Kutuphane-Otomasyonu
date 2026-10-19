using Library.Business.Dtos;
using Library.Domain.Common;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;

    public ReportService(IUnitOfWork uow) => _uow = uow;

    public Task<IReadOnlyList<BookBorrowStat>> MostBorrowedBooksAsync(
        int limit = 10,
        CancellationToken cancellationToken = default) =>
        _uow.Books.GetMostBorrowedAsync(limit, cancellationToken);

    public Task<IReadOnlyList<MemberBorrowStat>> TopBorrowingMembersAsync(
        int limit = 10,
        CancellationToken cancellationToken = default) =>
        _uow.Members.GetTopBorrowersAsync(limit, cancellationToken);

    public async Task<IReadOnlyList<LoanListItemDto>> ActiveLoansAsync(
        CancellationToken cancellationToken = default)
    {
        var loans = await _uow.Loans.GetActiveAsync(cancellationToken);
        return loans.Select(DashboardService.MapLoan).ToList();
    }

    public async Task<IReadOnlyList<LoanListItemDto>> OverdueLoansAsync(
        CancellationToken cancellationToken = default)
    {
        var loans = await _uow.Loans.GetOverdueAsync(cancellationToken);
        return loans.Select(DashboardService.MapLoan).ToList();
    }

    public async Task<IReadOnlyList<LoanListItemDto>> LoansByDateRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var loans = await _uow.Loans.GetByDateRangeAsync(from, to, cancellationToken);
        return loans.Select(DashboardService.MapLoan).ToList();
    }
}
