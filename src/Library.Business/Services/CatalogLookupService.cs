using Library.Domain.Entities;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class CatalogLookupService : ICatalogLookupService
{
    private readonly IUnitOfWork _uow;

    public CatalogLookupService(IUnitOfWork uow) => _uow = uow;

    public Task<IReadOnlyList<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default) =>
        _uow.Categories.ListAsync(cancellationToken);

    public Task<IReadOnlyList<Publisher>> ListPublishersAsync(CancellationToken cancellationToken = default) =>
        _uow.Publishers.ListAsync(cancellationToken);

    public Task<IReadOnlyList<Author>> ListAuthorsAsync(CancellationToken cancellationToken = default) =>
        _uow.Authors.ListAsync(cancellationToken);
}
