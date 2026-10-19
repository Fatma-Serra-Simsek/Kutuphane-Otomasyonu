using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Library.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public BookRepository(LibraryDbContext db) => _db = db;

    public Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Books
            .Include(b => b.Copies)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public void Add(Book book) => _db.Books.Add(book);

    public void Update(Book book) => _db.Books.Update(book);

    public async Task<PagedResult<Book>> SearchAsync(
        SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? SearchQuery.DefaultPageSize : query.PageSize;

        var books = _db.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Term))
        {
            var term = query.Term.Trim();
            books = books.Where(b =>
                EF.Functions.Like(b.Title, $"%{term}%") ||
                EF.Functions.Like(b.ISBN, $"%{term}%") ||
                b.BookAuthors.Any(ba => EF.Functions.Like(ba.Author.FullName, $"%{term}%")));
        }

        if (query.CategoryId is int categoryId)
            books = books.Where(b => b.CategoryId == categoryId);

        var totalCount = await books.CountAsync(cancellationToken);
        var items = await books
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.Copies)
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Book>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        _db.Books.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<BookBorrowStat>> GetMostBorrowedAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        var take = limit < 1 ? 10 : limit;
        return await _db.Loans
            .AsNoTracking()
            .Where(l => !l.BookCopy.IsDeleted && !l.BookCopy.Book.IsDeleted)
            .GroupBy(l => new { l.BookCopy.BookId, l.BookCopy.Book.Title })
            .Select(g => new BookBorrowStat
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}

