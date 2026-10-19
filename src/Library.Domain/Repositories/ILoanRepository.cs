using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetActiveByCopyAsync(int copyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CountOverdueAsync(CancellationToken cancellationToken = default);
    void Add(Loan loan);
    void Update(Loan loan);
}
