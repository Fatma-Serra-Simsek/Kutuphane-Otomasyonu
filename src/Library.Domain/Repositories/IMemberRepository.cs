using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Member member);
    void Update(Member member);
    Task<PagedResult<Member>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MemberBorrowStat>> GetTopBorrowersAsync(int limit, CancellationToken cancellationToken = default);
}
