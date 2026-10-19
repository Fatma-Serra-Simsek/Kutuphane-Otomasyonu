using Library.Domain.Entities;

namespace Library.Business.Services;

public interface ICatalogLookupService
{
    Task<IReadOnlyList<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Publisher>> ListPublishersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Author>> ListAuthorsAsync(CancellationToken cancellationToken = default);
}
