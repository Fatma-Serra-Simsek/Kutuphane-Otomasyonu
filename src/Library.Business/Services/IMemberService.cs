using Library.Business.Dtos;
using Library.Business.Results;
using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.Business.Services;

public interface IMemberService
{
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<Member>> CreateAsync(MemberCreateDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<Member>> UpdateAsync(MemberUpdateDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Member>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetLoansAsync(int memberId, CancellationToken cancellationToken = default);
}
