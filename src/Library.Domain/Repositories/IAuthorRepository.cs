using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Author>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Author author);
}
