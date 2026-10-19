using Library.Business.Dtos;
using Library.Domain.Common;

namespace Library.Business.Services;

public interface IReportService
{
    Task<IReadOnlyList<BookBorrowStat>> MostBorrowedBooksAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MemberBorrowStat>> TopBorrowingMembersAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanListItemDto>> ActiveLoansAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanListItemDto>> OverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanListItemDto>> LoansByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
