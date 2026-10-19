using FluentValidation;
using Library.Business.Dtos;
using Library.Business.Results;
using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<BookCreateDto> _createValidator;
    private readonly IValidator<BookUpdateDto> _updateValidator;
    private readonly IValidator<BookCopyCreateDto> _copyValidator;

    public BookService(
        IUnitOfWork uow,
        IValidator<BookCreateDto> createValidator,
        IValidator<BookUpdateDto> updateValidator,
        IValidator<BookCopyCreateDto> copyValidator)
    {
        _uow = uow;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _copyValidator = copyValidator;
    }

    public Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _uow.Books.GetByIdAsync(id, cancellationToken);

    public async Task<ServiceResult<Book>> CreateAsync(
        BookCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ServiceResult<Book>.Fail(validation.Errors[0].ErrorMessage);

        var book = new Book
        {
            Title = dto.Title.Trim(),
            ISBN = dto.ISBN.Trim(),
            PublishYear = dto.PublishYear,
            Description = dto.Description?.Trim() ?? string.Empty,
            CategoryId = dto.CategoryId,
            PublisherId = dto.PublisherId
        };

        foreach (var authorId in dto.AuthorIds.Distinct())
            book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });

        _uow.Books.Add(book);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<Book>.Ok(book);
    }

    public async Task<ServiceResult<Book>> UpdateAsync(
        BookUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ServiceResult<Book>.Fail(validation.Errors[0].ErrorMessage);

        var book = await _uow.Books.GetByIdAsync(dto.Id, cancellationToken);
        if (book is null)
            return ServiceResult<Book>.Fail("Kitap bulunamadı.");

        book.Title = dto.Title.Trim();
        book.ISBN = dto.ISBN.Trim();
        book.PublishYear = dto.PublishYear;
        book.Description = dto.Description?.Trim() ?? string.Empty;
        book.CategoryId = dto.CategoryId;
        book.PublisherId = dto.PublisherId;

        book.BookAuthors.Clear();
        foreach (var authorId in dto.AuthorIds.Distinct())
            book.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = authorId });

        _uow.Books.Update(book);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<Book>.Ok(book);
    }

    public async Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _uow.Books.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return ServiceResult.Fail("Kitap bulunamadı.");

        var hasActiveLoan = book.Copies.Any(c =>
            !c.IsDeleted && c.Status == CopyStatus.Loaned);
        if (hasActiveLoan)
            return ServiceResult.Fail("Ödünçteki kopyası olan kitap silinemez.");

        book.IsDeleted = true;
        foreach (var copy in book.Copies.Where(c => !c.IsDeleted))
            copy.IsDeleted = true;

        _uow.Books.Update(book);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }

    public Task<PagedResult<Book>> SearchAsync(
        SearchQuery query,
        CancellationToken cancellationToken = default) =>
        _uow.Books.SearchAsync(query, cancellationToken);

    public async Task<ServiceResult<BookCopy>> AddCopyAsync(
        BookCopyCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _copyValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ServiceResult<BookCopy>.Fail(validation.Errors[0].ErrorMessage);

        var book = await _uow.Books.GetByIdAsync(dto.BookId, cancellationToken);
        if (book is null)
            return ServiceResult<BookCopy>.Fail("Kitap bulunamadı.");

        var existing = await _uow.BookCopies.GetByBarcodeAsync(dto.Barcode.Trim(), cancellationToken);
        if (existing is not null)
            return ServiceResult<BookCopy>.Fail("Bu barkod zaten kayıtlı.");

        var copy = new BookCopy
        {
            BookId = dto.BookId,
            Barcode = dto.Barcode.Trim(),
            Status = CopyStatus.Available
        };

        _uow.BookCopies.Add(copy);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<BookCopy>.Ok(copy);
    }

    public async Task<ServiceResult<BookCopy>> ChangeCopyStatusAsync(
        int copyId,
        CopyStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var copy = await _uow.BookCopies.GetByIdAsync(copyId, cancellationToken);
        if (copy is null)
            return ServiceResult<BookCopy>.Fail("Kopya bulunamadı.");

        if (!CopyStatusTransitions.CanTransition(copy.Status, newStatus))
            return ServiceResult<BookCopy>.Fail(
                $"{copy.Status} durumundan {newStatus} durumuna geçiş yapılamaz.");

        if (copy.Status == CopyStatus.Loaned && newStatus != CopyStatus.Lost)
        {
            var active = await _uow.Loans.GetActiveByCopyAsync(copyId, cancellationToken);
            if (active is not null && newStatus == CopyStatus.Available)
                return ServiceResult<BookCopy>.Fail("Aktif ödünç varken durum Available yapılamaz; iade kullanın.");
        }

        copy.Status = newStatus;
        _uow.BookCopies.Update(copy);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<BookCopy>.Ok(copy);
    }
}
