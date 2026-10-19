using Library.Business.Dtos;
using Library.Business.Results;
using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Enums;

namespace Library.Business.Services;

public interface IBookService
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<Book>> CreateAsync(BookCreateDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<Book>> UpdateAsync(BookUpdateDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<ServiceResult<BookCopy>> AddCopyAsync(BookCopyCreateDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<BookCopy>> ChangeCopyStatusAsync(int copyId, CopyStatus newStatus, CancellationToken cancellationToken = default);
}
