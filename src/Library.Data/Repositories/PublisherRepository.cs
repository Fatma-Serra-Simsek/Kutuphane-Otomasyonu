using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly LibraryDbContext _db;

    public PublisherRepository(LibraryDbContext db) => _db = db;

    public Task<Publisher?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Publishers.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Publisher>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Publishers.AsNoTracking().OrderBy(p => p.Name).ToListAsync(cancellationToken);

    public void Add(Publisher publisher) => _db.Publishers.Add(publisher);
}
