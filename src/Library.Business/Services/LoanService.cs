using Library.Business.Results;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class LoanService : ILoanService
{
    public const string LoanPeriodDaysKey = "LoanPeriodDays";
    public const int DefaultLoanPeriodDays = 14;

    private readonly IUnitOfWork _uow;

    public LoanService(IUnitOfWork uow) => _uow = uow;

    public async Task<LoanResult> BorrowBookAsync(
        int memberId,
        int copyId,
        int staffId,
        CancellationToken cancellationToken = default)
    {
        var member = await _uow.Members.GetByIdAsync(memberId, cancellationToken);
        if (member is null)
            return LoanResult.Fail(LoanResultCode.MemberNotFound, "Üye bulunamadı.");

        if (!member.CanBorrow())
            return LoanResult.Fail(LoanResultCode.MemberInactive, "Üye aktif değil.");

        var copy = await _uow.BookCopies.GetByIdAsync(copyId, cancellationToken);
        if (copy is null)
            return LoanResult.Fail(LoanResultCode.CopyNotFound, "Kopya bulunamadı.");

        if (copy.Status != CopyStatus.Available)
            return LoanResult.Fail(LoanResultCode.CopyNotAvailable, "Kopya müsait değil.");

        var existingActive = await _uow.Loans.GetActiveByCopyAsync(copyId, cancellationToken);
        if (existingActive is not null)
            return LoanResult.Fail(LoanResultCode.CopyNotAvailable, "Kopyanın aktif ödüncü var.");

        var periodDays = await ResolveLoanPeriodDaysAsync(cancellationToken);
        var loanDate = DateTime.Today;
        var loan = new Loan
        {
            MemberId = memberId,
            BookCopyId = copyId,
            StaffId = staffId,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(periodDays),
            ReturnDate = null
        };

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            copy.Status = CopyStatus.Loaned;
            _uow.BookCopies.Update(copy);
            _uow.Loans.Add(loan);
            await _uow.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return LoanResult.Ok(loan);
        }
        catch (Exception)
        {
            // Dispose rollbacks uncommitted tx. UX_Loans_ActiveCopy / concurrency → CopyNotAvailable.
            return LoanResult.Fail(LoanResultCode.CopyNotAvailable, "Kopya aynı anda ödünç verilemez.");
        }
    }


    public async Task<LoanResult> BorrowByBarcodeAsync(
        int memberId,
        string barcode,
        int staffId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return LoanResult.Fail(LoanResultCode.CopyNotFound, "Barkod gerekli.");

        var copy = await _uow.BookCopies.GetByBarcodeAsync(barcode.Trim(), cancellationToken);
        if (copy is null)
            return LoanResult.Fail(LoanResultCode.CopyNotFound, "Kopya bulunamadı.");

        return await BorrowBookAsync(memberId, copy.Id, staffId, cancellationToken);
    }

    public async Task<ReturnResult> ReturnBookAsync(
        int copyId,
        CancellationToken cancellationToken = default)
    {
        var loan = await _uow.Loans.GetActiveByCopyAsync(copyId, cancellationToken);
        if (loan is null)
            return ReturnResult.Fail(LoanResultCode.NoActiveLoan, "Aktif ödünç bulunamadı.");

        var copy = await _uow.BookCopies.GetByIdAsync(copyId, cancellationToken);
        if (copy is null)
            return ReturnResult.Fail(LoanResultCode.CopyNotFound, "Kopya bulunamadı.");

        var returnDate = DateTime.Today;
        var daysOverdue = Math.Max(0, (returnDate - loan.DueDate.Date).Days);

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        loan.ReturnDate = returnDate;
        copy.Status = CopyStatus.Available;
        _uow.Loans.Update(loan);
        _uow.BookCopies.Update(copy);
        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return ReturnResult.Ok(loan, daysOverdue);
    }

    public async Task<ReturnResult> ReturnByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return ReturnResult.Fail(LoanResultCode.CopyNotFound, "Barkod gerekli.");

        var copy = await _uow.BookCopies.GetByBarcodeAsync(barcode.Trim(), cancellationToken);
        if (copy is null)
            return ReturnResult.Fail(LoanResultCode.CopyNotFound, "Kopya bulunamadı.");

        return await ReturnBookAsync(copy.Id, cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return null;

        var copy = await _uow.BookCopies.GetByBarcodeAsync(barcode.Trim(), cancellationToken);
        if (copy is null)
            return null;

        return await _uow.Loans.GetActiveByCopyAsync(copy.Id, cancellationToken);
    }

    public Task<IReadOnlyList<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default) =>
        _uow.Loans.GetActiveAsync(cancellationToken);

    public Task<IReadOnlyList<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default) =>
        _uow.Loans.GetOverdueAsync(cancellationToken);

    private async Task<int> ResolveLoanPeriodDaysAsync(CancellationToken cancellationToken)
    {
        var value = await _uow.Settings.GetValueAsync(LoanPeriodDaysKey, cancellationToken);
        return int.TryParse(value, out var days) && days > 0 ? days : DefaultLoanPeriodDays;
    }
}
