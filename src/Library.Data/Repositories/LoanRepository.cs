using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _db;

    public LoanRepository(LibraryDbContext db) => _db = db;

    public Task<Loan?> GetActiveByCopyAsync(int copyId, CancellationToken cancellationToken = default) =>
        _db.Loans
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .FirstOrDefaultAsync(l => l.BookCopyId == copyId && l.ReturnDate == null, cancellationToken);

    public async Task<IReadOnlyList<Loan>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .Where(l => l.ReturnDate == null)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Loan>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .Where(l => l.ReturnDate == null && l.DueDate < today)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetRecentAsync(int count, CancellationToken cancellationToken = default) =>
        await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .OrderByDescending(l => l.LoanDate)
            .ThenByDescending(l => l.Id)
            .Take(count < 1 ? 10 : count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Loan>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default) =>
        await _db.Loans
            .AsNoTracking()
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .Include(l => l.Staff)
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Loan>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var fromDate = from.Date;
        var toDate = to.Date;
        return await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(c => c.Book)
            .Where(l => l.LoanDate >= fromDate && l.LoanDate <= toDate)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default) =>
        _db.Loans.CountAsync(l => l.ReturnDate == null, cancellationToken);

    public Task<int> CountOverdueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return _db.Loans.CountAsync(l => l.ReturnDate == null && l.DueDate < today, cancellationToken);
    }

    public void Add(Loan loan) => _db.Loans.Add(loan);

    public void Update(Loan loan) => _db.Loans.Update(loan);
}
