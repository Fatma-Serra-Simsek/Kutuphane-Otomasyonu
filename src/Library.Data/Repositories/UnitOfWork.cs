using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Library.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _db;

    public UnitOfWork(
        LibraryDbContext db,
        IBookRepository books,
        IBookCopyRepository bookCopies,
        IMemberRepository members,
        ILoanRepository loans,
        IAuthorRepository authors,
        IPublisherRepository publishers,
        ICategoryRepository categories,
        ISettingRepository settings,
        IStaffRepository staff)
    {
        _db = db;
        Books = books;
        BookCopies = bookCopies;
        Members = members;
        Loans = loans;
        Authors = authors;
        Publishers = publishers;
        Categories = categories;
        Settings = settings;
        Staff = staff;
    }

    public IBookRepository Books { get; }
    public IBookCopyRepository BookCopies { get; }
    public IMemberRepository Members { get; }
    public ILoanRepository Loans { get; }
    public IAuthorRepository Authors { get; }
    public IPublisherRepository Publishers { get; }
    public ICategoryRepository Categories { get; }
    public ISettingRepository Settings { get; }
    public IStaffRepository Staff { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        return new UnitOfWorkTransaction(transaction);
    }

    private sealed class UnitOfWorkTransaction : IUnitOfWorkTransaction
    {
        private readonly IDbContextTransaction _transaction;
        private bool _committed;

        public UnitOfWorkTransaction(IDbContextTransaction transaction) =>
            _transaction = transaction;

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_committed)
                await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();
        }
    }
}
