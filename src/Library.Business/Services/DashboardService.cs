using Library.Business.Dtos;
using Library.Domain.Entities;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalBooks = await _uow.Books.CountAsync(cancellationToken);
        var activeMembers = await _uow.Members.CountActiveAsync(cancellationToken);
        var activeLoans = await _uow.Loans.CountActiveAsync(cancellationToken);
        var overdueLoans = await _uow.Loans.CountOverdueAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalBooks = totalBooks,
            ActiveMembers = activeMembers,
            ActiveLoans = activeLoans,
            OverdueLoans = overdueLoans
        };
    }

    public async Task<IReadOnlyList<LoanListItemDto>> GetRecentLoansAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        var loans = await _uow.Loans.GetRecentAsync(count, cancellationToken);
        return loans.Select(MapLoan).ToList();
    }

    internal static LoanListItemDto MapLoan(Loan loan) => new()
    {
        LoanId = loan.Id,
        BookTitle = loan.BookCopy?.Book?.Title ?? string.Empty,
        Barcode = loan.BookCopy?.Barcode ?? string.Empty,
        MemberName = loan.Member?.FullName ?? string.Empty,
        MemberNumber = loan.Member?.MemberNumber ?? string.Empty,
        LoanDate = loan.LoanDate,
        DueDate = loan.DueDate,
        ReturnDate = loan.ReturnDate,
        IsOverdue = loan.IsOverdue()
    };
}
