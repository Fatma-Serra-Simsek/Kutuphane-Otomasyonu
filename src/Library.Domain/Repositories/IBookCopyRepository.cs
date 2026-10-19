using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IBookCopyRepository
{
    Task<BookCopy?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BookCopy?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    void Add(BookCopy copy);
    void Update(BookCopy copy);
}
