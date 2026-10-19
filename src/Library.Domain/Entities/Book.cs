using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }
    public bool IsDeleted { get; set; }

    public Category Category { get; set; } = null!;
    public Publisher Publisher { get; set; } = null!;
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public int GetAvailableCopyCount() =>
        Copies.Count(c => !c.IsDeleted && c.Status == CopyStatus.Available);
}
