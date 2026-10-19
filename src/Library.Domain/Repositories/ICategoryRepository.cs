using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Category category);
}
