namespace Library.Domain.Repositories;

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IBookCopyRepository BookCopies { get; }
    IMemberRepository Members { get; }
    ILoanRepository Loans { get; }
    IAuthorRepository Authors { get; }
    IPublisherRepository Publishers { get; }
    ICategoryRepository Categories { get; }
    ISettingRepository Settings { get; }
    IStaffRepository Staff { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

