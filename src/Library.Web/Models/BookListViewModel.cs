using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.Web.Models;

public class BookListViewModel
{
    public string? Term { get; init; }
    public int? CategoryId { get; init; }
    public PagedResult<Book> Result { get; init; } = new();
}
