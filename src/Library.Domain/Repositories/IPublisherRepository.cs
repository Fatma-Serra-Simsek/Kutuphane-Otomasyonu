using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IPublisherRepository
{
    Task<Publisher?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Publisher>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Publisher publisher);
}
