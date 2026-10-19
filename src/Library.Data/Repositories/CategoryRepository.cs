using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly LibraryDbContext _db;

    public CategoryRepository(LibraryDbContext db) => _db = db;

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public void Add(Category category) => _db.Categories.Add(category);
}
