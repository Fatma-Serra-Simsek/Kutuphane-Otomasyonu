namespace Library.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
