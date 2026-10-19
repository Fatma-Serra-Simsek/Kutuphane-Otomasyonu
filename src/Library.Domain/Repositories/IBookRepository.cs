using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Book book);
    void Update(Book book);
    Task<PagedResult<Book>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookBorrowStat>> GetMostBorrowedAsync(int limit, CancellationToken cancellationToken = default);
}
