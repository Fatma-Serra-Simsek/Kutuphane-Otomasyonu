using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _db;

    public AuthorRepository(LibraryDbContext db) => _db = db;

    public Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Author>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Authors.AsNoTracking().OrderBy(a => a.FullName).ToListAsync(cancellationToken);

    public void Add(Author author) => _db.Authors.Add(author);
}
