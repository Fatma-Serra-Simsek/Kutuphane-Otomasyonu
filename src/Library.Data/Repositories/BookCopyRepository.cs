using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class BookCopyRepository : IBookCopyRepository
{
    private readonly LibraryDbContext _db;

    public BookCopyRepository(LibraryDbContext db) => _db = db;

    public Task<BookCopy?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.BookCopies
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<BookCopy?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default) =>
        _db.BookCopies
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Barcode == barcode, cancellationToken);

    public void Add(BookCopy copy) => _db.BookCopies.Add(copy);

    public void Update(BookCopy copy) => _db.BookCopies.Update(copy);
}
