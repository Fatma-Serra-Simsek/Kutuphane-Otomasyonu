using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Library.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _db;

    public MemberRepository(LibraryDbContext db) => _db = db;

    public Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Members.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public void Add(Member member) => _db.Members.Add(member);

    public void Update(Member member) => _db.Members.Update(member);

    public async Task<PagedResult<Member>> SearchAsync(
        SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? SearchQuery.DefaultPageSize : query.PageSize;

        var members = _db.Members.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Term))
        {
            var term = query.Term.Trim();
            members = members.Where(m =>
                EF.Functions.Like(m.FullName, $"%{term}%") ||
                EF.Functions.Like(m.MemberNumber, $"%{term}%") ||
                EF.Functions.Like(m.Email, $"%{term}%"));
        }

        if (query.IsActive is bool isActive)
            members = members.Where(m => m.IsActive == isActive);

        var totalCount = await members.CountAsync(cancellationToken);
        var items = await members
            .OrderBy(m => m.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Member>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default) =>
        _db.Members.CountAsync(m => m.IsActive, cancellationToken);

    public async Task<IReadOnlyList<MemberBorrowStat>> GetTopBorrowersAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        var take = limit < 1 ? 10 : limit;
        return await _db.Loans
            .AsNoTracking()
            .GroupBy(l => new { l.MemberId, l.Member.FullName, l.Member.MemberNumber })
            .Select(g => new MemberBorrowStat
            {
                MemberId = g.Key.MemberId,
                FullName = g.Key.FullName,
                MemberNumber = g.Key.MemberNumber,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}

