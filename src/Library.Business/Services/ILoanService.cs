using Library.Business.Results;
using Library.Domain.Entities;

namespace Library.Business.Services;

public interface ILoanService
{
    Task<LoanResult> BorrowBookAsync(int memberId, int copyId, int staffId, CancellationToken cancellationToken = default);
    Task<LoanResult> BorrowByBarcodeAsync(int memberId, string barcode, int staffId, CancellationToken cancellationToken = default);
    Task<ReturnResult> ReturnBookAsync(int copyId, CancellationToken cancellationToken = default);
    Task<ReturnResult> ReturnByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<Loan?> GetActiveLoanByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
}
